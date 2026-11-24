using DinoCars.AppLogic.Listings;
using DinoCars.Domain.Listings;

namespace DinoCars.Web.Components.Pages.Listings
{
    /// <summary>
    /// Translates between the overview's URL (?merk=BMW&amp;brandstof=diesel&amp;maxprijs=20000&amp;sorteer=prijs-laag)
    /// and a <see cref="ListingSearch"/>. Dutch, readable query values, so links can be shared.
    /// </summary>
    public static class ListingFilterOptions
    {
        public static readonly IReadOnlyList<int> MaxPrices = [5_000, 10_000, 15_000, 20_000, 25_000, 30_000, 40_000, 50_000];

        public static readonly IReadOnlyList<(string Value, string Label, ListingSort Sort)> Sorts =
        [
            ("nieuwste", "Nieuwste eerst", ListingSort.Newest),
            ("prijs-laag", "Prijs: laag naar hoog", ListingSort.PriceLowToHigh),
            ("prijs-hoog", "Prijs: hoog naar laag", ListingSort.PriceHighToLow),
            ("km-laag", "Kilometerstand: laag naar hoog", ListingSort.MileageLowToHigh)
        ];

        public static IEnumerable<FuelType> FuelTypes => Enum.GetValues<FuelType>();

        public static string FuelValue(FuelType fuelType) => fuelType.ToString().ToLowerInvariant();

        public static ListingSearch ToSearch(string? make, string? fuel, int? maxPrice, string? sort) => new(
            Make: string.IsNullOrWhiteSpace(make) ? null : make.Trim(),
            FuelType: Enum.TryParse<FuelType>(fuel, ignoreCase: true, out var fuelType) && Enum.IsDefined(fuelType) ? fuelType : null,
            MaxPrice: maxPrice is > 0 ? maxPrice : null,
            Sort: ParseSort(sort));

        private static ListingSort ParseSort(string? value)
        {
            foreach (var option in Sorts)
            {
                if (option.Value == value)
                    return option.Sort;
            }

            return ListingSort.Newest;
        }
    }
}
