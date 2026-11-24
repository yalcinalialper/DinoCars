using DinoCars.AppLogic.Availability;
using DinoCars.Domain.Listings;
using Microsoft.Extensions.Options;

namespace DinoCars.AppLogic.Listings
{
    /// <summary>Converts CarListing entities into the read models the UI uses, including photo URLs.</summary>
    internal sealed class ListingViewMapper(IImageStorage imageStorage, IOptions<WorkshopOptions> options)
    {
        private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId);

        public ListingCardView ToCard(CarListing listing) => new(
            listing.Reference,
            listing.Slug,
            listing.Title,
            listing.Year,
            listing.Mileage,
            listing.Price,
            listing.FuelType,
            listing.Transmission,
            listing.Status,
            CoverThumbnailUrl(listing));

        public ListingDetailView ToDetail(CarListing listing) => new(
            listing.Reference,
            listing.Slug,
            listing.Title,
            listing.Make,
            listing.Model,
            listing.Version,
            listing.Year,
            listing.Mileage,
            listing.Price,
            listing.FuelType,
            listing.Transmission,
            listing.PowerKw,
            listing.Color,
            listing.Description,
            listing.Status,
            ToPhotoViews(listing));

        public ListingAdminRow ToAdminRow(CarListing listing) => new(
            listing.Id,
            listing.Reference,
            listing.Slug,
            listing.Title,
            listing.Year,
            listing.Price,
            listing.Status,
            listing.Photos.Count,
            CoverThumbnailUrl(listing),
            TimeZoneInfo.ConvertTimeFromUtc(listing.UpdatedAtUtc, _timeZone));

        public ListingEditView ToEditView(CarListing listing) => new(
            listing.Id,
            listing.Reference,
            listing.Slug,
            listing.Status,
            new ListingDetails(
                listing.Make,
                listing.Model,
                listing.Version,
                listing.Year,
                listing.Mileage,
                listing.Price,
                listing.FuelType,
                listing.Transmission,
                listing.PowerKw,
                listing.Color,
                listing.Description),
            ToPhotoViews(listing));

        private IReadOnlyList<PhotoView> ToPhotoViews(CarListing listing) =>
            listing.Photos
                .OrderBy(photo => photo.SortOrder)
                .Select(photo => new PhotoView(
                    photo.Id,
                    imageStorage.GetPublicUrl(ListingPhotoFiles.Large(listing.Id, photo.Id)),
                    imageStorage.GetPublicUrl(ListingPhotoFiles.Thumbnail(listing.Id, photo.Id)),
                    photo.Width,
                    photo.Height))
                .ToList();

        private string? CoverThumbnailUrl(CarListing listing) =>
            listing.CoverPhoto is { } cover
                ? imageStorage.GetPublicUrl(ListingPhotoFiles.Thumbnail(listing.Id, cover.Id))
                : null;
    }
}
