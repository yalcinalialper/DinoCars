using System.ComponentModel.DataAnnotations;
using DinoCars.Domain.Listings;

namespace DinoCars.Web.Components.Pages.Admin.Listings
{
    /// <summary>
    /// Editable copy of a car's details for the back-office form. These annotations give instant feedback
    /// in the form; the domain (<see cref="CarListing"/>) still checks every rule when saving.
    /// </summary>
    public sealed class ListingForm
    {
        [Required(ErrorMessage = "Merk is verplicht.")]
        [MaxLength(CarListing.MaxNameLength)]
        public string Make { get; set; } = "";

        [Required(ErrorMessage = "Model is verplicht.")]
        [MaxLength(CarListing.MaxNameLength)]
        public string Model { get; set; } = "";

        [MaxLength(CarListing.MaxVersionLength)]
        public string? Version { get; set; }

        [Required(ErrorMessage = "Bouwjaar is verplicht.")]
        [Range(1950, 2100, ErrorMessage = "Geef een geldig bouwjaar in.")]
        public int? Year { get; set; }

        [Required(ErrorMessage = "Kilometerstand is verplicht.")]
        [Range(0, 2_000_000, ErrorMessage = "Geef een geldige kilometerstand in.")]
        public int? Mileage { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht.")]
        [Range(typeof(decimal), "1", "10000000", ParseLimitsInInvariantCulture = true, ErrorMessage = "Geef een geldige prijs in.")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Kies een brandstof.")]
        public FuelType? FuelType { get; set; }

        [Required(ErrorMessage = "Kies een versnellingsbak.")]
        public Transmission? Transmission { get; set; }

        [Range(1, 2000, ErrorMessage = "Geef een geldig vermogen in kW.")]
        public int? PowerKw { get; set; }

        [MaxLength(CarListing.MaxColorLength)]
        public string? Color { get; set; }

        [MaxLength(CarListing.MaxDescriptionLength, ErrorMessage = "De omschrijving is te lang.")]
        public string? Description { get; set; }

        public static ListingForm From(ListingDetails details) => new()
        {
            Make = details.Make,
            Model = details.Model,
            Version = details.Version,
            Year = details.Year,
            Mileage = details.Mileage,
            Price = details.Price,
            FuelType = details.FuelType,
            Transmission = details.Transmission,
            PowerKw = details.PowerKw,
            Color = details.Color,
            Description = details.Description
        };

        /// <summary>Only call after validation succeeded: the required values are then filled in.</summary>
        public ListingDetails ToDetails() => new(
            Make.Trim(),
            Model.Trim(),
            NullIfEmpty(Version),
            Year!.Value,
            Mileage!.Value,
            Price!.Value,
            FuelType!.Value,
            Transmission!.Value,
            PowerKw,
            NullIfEmpty(Color),
            NullIfEmpty(Description));

        private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
