using DinoCars.Domain.Listings;

namespace DinoCars.AppLogic.Listings
{
    /// <summary>Filters and sorting for the public car overview. Every filter is optional.</summary>
    public sealed record ListingSearch(
        string? Make = null,
        FuelType? FuelType = null,
        decimal? MaxPrice = null,
        ListingSort Sort = ListingSort.Newest);

    public enum ListingSort
    {
        Newest = 0,
        PriceLowToHigh = 1,
        PriceHighToLow = 2,
        MileageLowToHigh = 3
    }
}
