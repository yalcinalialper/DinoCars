using DinoCars.Domain.Listings;

namespace DinoCars.AppLogic.Listings
{
    // Read models for the UI. Pages never receive CarListing entities, so they can't change them by accident.

    public sealed record PhotoView(Guid Id, string LargeUrl, string ThumbnailUrl, int Width, int Height);

    /// <summary>A car in the public overview grid.</summary>
    public sealed record ListingCardView(
        string Reference,
        string Slug,
        string Title,
        int Year,
        int Mileage,
        decimal Price,
        FuelType FuelType,
        Transmission Transmission,
        ListingStatus Status,
        string? CoverUrl);

    /// <summary>Everything shown on the public detail page of a car.</summary>
    public sealed record ListingDetailView(
        string Reference,
        string Slug,
        string Title,
        string Make,
        string Model,
        string? Version,
        int Year,
        int Mileage,
        decimal Price,
        FuelType FuelType,
        Transmission Transmission,
        int? PowerKw,
        string? Color,
        string? Description,
        ListingStatus Status,
        IReadOnlyList<PhotoView> Photos);

    /// <summary>A public page for the sitemap.</summary>
    public sealed record ListingLink(string Reference, string Slug, DateTime UpdatedAtUtc);

    /// <summary>A row in the back-office list.</summary>
    public sealed record ListingAdminRow(
        Guid Id,
        string Reference,
        string Slug,
        string Title,
        int Year,
        decimal Price,
        ListingStatus Status,
        int PhotoCount,
        string? ThumbnailUrl,
        DateTime UpdatedAtUtc);

    /// <summary>A listing as the back-office editor needs it.</summary>
    public sealed record ListingEditView(
        Guid Id,
        string Reference,
        string Slug,
        ListingStatus Status,
        ListingDetails Details,
        IReadOnlyList<PhotoView> Photos);
}
