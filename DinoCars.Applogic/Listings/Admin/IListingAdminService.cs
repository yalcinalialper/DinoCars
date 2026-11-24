using DinoCars.Domain.Listings;

namespace DinoCars.AppLogic.Listings.Admin
{
    /// <summary>Back-office use cases for managing the cars for sale.</summary>
    public interface IListingAdminService
    {
        Task<IReadOnlyList<ListingAdminRow>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<ListingEditView?> FindForEditAsync(Guid listingId, CancellationToken cancellationToken = default);

        /// <summary>Creates a new listing as a draft and returns its id.</summary>
        Task<Guid> CreateAsync(ListingDetails details, CancellationToken cancellationToken = default);

        Task UpdateDetailsAsync(Guid listingId, ListingDetails details, CancellationToken cancellationToken = default);

        Task DeleteAsync(Guid listingId, CancellationToken cancellationToken = default);

        // Photos
        Task AddPhotoAsync(Guid listingId, Stream photo, CancellationToken cancellationToken = default);
        Task RemovePhotoAsync(Guid listingId, Guid photoId, CancellationToken cancellationToken = default);
        Task MovePhotoAsync(Guid listingId, Guid photoId, int offset, CancellationToken cancellationToken = default);
        Task SetCoverPhotoAsync(Guid listingId, Guid photoId, CancellationToken cancellationToken = default);

        // Status
        Task PublishAsync(Guid listingId, CancellationToken cancellationToken = default);
        Task ReserveAsync(Guid listingId, CancellationToken cancellationToken = default);
        Task MarkSoldAsync(Guid listingId, CancellationToken cancellationToken = default);
        Task UnpublishAsync(Guid listingId, CancellationToken cancellationToken = default);
    }
}
