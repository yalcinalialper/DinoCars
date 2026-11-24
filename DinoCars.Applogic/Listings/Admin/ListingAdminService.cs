using DinoCars.Domain.Exceptions;
using DinoCars.Domain.Listings;

namespace DinoCars.AppLogic.Listings.Admin
{
    internal sealed class ListingAdminService(
        IListingRepository listings,
        IImageProcessor imageProcessor,
        IImageStorage imageStorage,
        ListingViewMapper mapper,
        IUnitOfWork unitOfWork,
        IClock clock) : IListingAdminService
    {
        public async Task<IReadOnlyList<ListingAdminRow>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var all = await listings.GetAllAsync(cancellationToken);
            return all.Select(mapper.ToAdminRow).ToList();
        }

        public async Task<ListingEditView?> FindForEditAsync(Guid listingId, CancellationToken cancellationToken = default)
        {
            var listing = await listings.GetByIdAsync(listingId, cancellationToken);
            return listing is null ? null : mapper.ToEditView(listing);
        }

        public async Task<Guid> CreateAsync(ListingDetails details, CancellationToken cancellationToken = default)
        {
            var listing = CarListing.Create(details, clock.UtcNow);

            listings.Add(listing);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return listing.Id;
        }

        public Task UpdateDetailsAsync(Guid listingId, ListingDetails details, CancellationToken cancellationToken = default) =>
            ChangeAsync(listingId, listing => listing.UpdateDetails(details, clock.UtcNow), cancellationToken);

        public async Task DeleteAsync(Guid listingId, CancellationToken cancellationToken = default)
        {
            var listing = await GetRequiredAsync(listingId, cancellationToken);
            var photoIds = listing.Photos.Select(photo => photo.Id).ToList();

            listings.Remove(listing);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Files last: if the database delete fails, the photos are still there.
            foreach (var photoId in photoIds)
                await DeletePhotoFilesAsync(listingId, photoId, cancellationToken);
        }

        // ----- Photos -----

        public async Task AddPhotoAsync(Guid listingId, Stream photo, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(photo);

            var listing = await GetRequiredAsync(listingId, cancellationToken);
            var image = await ProcessAsync(photo, cancellationToken);
            var listingPhoto = listing.AddPhoto(image.Large.Width, image.Large.Height, clock.UtcNow);

            await imageStorage.SaveAsync(
                ListingPhotoFiles.Large(listing.Id, listingPhoto.Id), image.Large.Content, image.Large.ContentType, cancellationToken);
            await imageStorage.SaveAsync(
                ListingPhotoFiles.Thumbnail(listing.Id, listingPhoto.Id), image.Thumbnail.Content, image.Thumbnail.ContentType, cancellationToken);

            try
            {
                await unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                // Don't leave files behind for a photo that was never saved.
                await DeletePhotoFilesAsync(listing.Id, listingPhoto.Id, CancellationToken.None);
                throw;
            }
        }

        public async Task RemovePhotoAsync(Guid listingId, Guid photoId, CancellationToken cancellationToken = default)
        {
            await ChangeAsync(listingId, listing => listing.RemovePhoto(photoId, clock.UtcNow), cancellationToken);
            await DeletePhotoFilesAsync(listingId, photoId, cancellationToken);
        }

        public Task MovePhotoAsync(Guid listingId, Guid photoId, int offset, CancellationToken cancellationToken = default) =>
            ChangeAsync(listingId, listing => listing.MovePhoto(photoId, offset, clock.UtcNow), cancellationToken);

        public Task SetCoverPhotoAsync(Guid listingId, Guid photoId, CancellationToken cancellationToken = default) =>
            ChangeAsync(listingId, listing => listing.SetCoverPhoto(photoId, clock.UtcNow), cancellationToken);

        // ----- Status -----

        public Task PublishAsync(Guid listingId, CancellationToken cancellationToken = default) =>
            ChangeAsync(listingId, listing => listing.Publish(clock.UtcNow), cancellationToken);

        public Task ReserveAsync(Guid listingId, CancellationToken cancellationToken = default) =>
            ChangeAsync(listingId, listing => listing.Reserve(clock.UtcNow), cancellationToken);

        public Task MarkSoldAsync(Guid listingId, CancellationToken cancellationToken = default) =>
            ChangeAsync(listingId, listing => listing.MarkSold(clock.UtcNow), cancellationToken);

        public Task UnpublishAsync(Guid listingId, CancellationToken cancellationToken = default) =>
            ChangeAsync(listingId, listing => listing.Unpublish(clock.UtcNow), cancellationToken);

        // ----- Helpers -----

        /// <summary>Load, let the domain apply the change (and its rules), save.</summary>
        private async Task ChangeAsync(Guid listingId, Action<CarListing> change, CancellationToken cancellationToken)
        {
            var listing = await GetRequiredAsync(listingId, cancellationToken);
            change(listing);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<CarListing> GetRequiredAsync(Guid listingId, CancellationToken cancellationToken) =>
            await listings.GetByIdAsync(listingId, cancellationToken)
            ?? throw new DomainException("Deze occasie bestaat niet (meer). Vernieuw de pagina.");

        private async Task<ProcessedImage> ProcessAsync(Stream photo, CancellationToken cancellationToken)
        {
            try
            {
                return await imageProcessor.ProcessAsync(photo, cancellationToken);
            }
            catch (InvalidImageException)
            {
                throw new DomainException("Dit bestand is geen geldige foto. Gebruik een JPG-, PNG- of WebP-foto.");
            }
        }

        private async Task DeletePhotoFilesAsync(Guid listingId, Guid photoId, CancellationToken cancellationToken)
        {
            foreach (var key in ListingPhotoFiles.All(listingId, photoId))
                await imageStorage.DeleteAsync(key, cancellationToken);
        }
    }
}
