using System.Globalization;
using DinoCars.Domain.Listings;

namespace DinoCars.Web.Helpers
{
    /// <summary>How listing data is shown to Dutch-speaking visitors. One place, so every page shows it the same way.</summary>
    public static class ListingFormat
    {
        private static readonly CultureInfo Dutch = new("nl-BE");
        private const double HorsepowerPerKw = 1.35962;

        public static string Price(decimal price) => price.ToString("C0", Dutch);

        public static string Mileage(int kilometers) => $"{kilometers.ToString("N0", Dutch)} km";

        /// <summary>"135 kW (184 pk)": Belgian buyers think in pk, the registration papers say kW.</summary>
        public static string Power(int kw) => $"{kw} kW ({Math.Round(kw * HorsepowerPerKw)} pk)";

        public static string FuelLabel(FuelType fuelType) => fuelType switch
        {
            FuelType.Petrol => "Benzine",
            FuelType.Diesel => "Diesel",
            FuelType.Hybrid => "Hybride",
            FuelType.PlugInHybrid => "Plug-in hybride",
            FuelType.Electric => "Elektrisch",
            FuelType.Lpg => "LPG",
            _ => "Andere"
        };

        public static string TransmissionLabel(Transmission transmission) => transmission switch
        {
            Transmission.Automatic => "Automaat",
            _ => "Manueel"
        };

        public static string StatusLabel(ListingStatus status) => status switch
        {
            ListingStatus.Draft => "Concept",
            ListingStatus.ForSale => "Te koop",
            ListingStatus.Reserved => "Gereserveerd",
            ListingStatus.Sold => "Verkocht",
            _ => status.ToString()
        };

        /// <summary>"2019 · 120.000 km · Diesel · Automaat"</summary>
        public static string Summary(int year, int mileage, FuelType fuelType, Transmission transmission) =>
            $"{year} · {Mileage(mileage)} · {FuelLabel(fuelType)} · {TransmissionLabel(transmission)}";

        /// <summary>Relative URL of a car's public page, e.g. "occasies/k3f9x2ab/bmw-320d-2018".</summary>
        public static string Url(string reference, string slug) => $"occasies/{reference}/{slug}";
    }
}
