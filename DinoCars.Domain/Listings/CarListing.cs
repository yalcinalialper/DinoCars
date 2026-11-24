using System.Security.Cryptography;
using DinoCars.Domain.Common;
using DinoCars.Domain.Exceptions;

namespace DinoCars.Domain.Listings
{
    /// <summary>
    /// A car the garage offers for sale. This is the aggregate root of the listings module:
    /// photos are only added, removed and ordered through this class, so its rules always hold.
    /// </summary>
    public sealed class CarListing
    {
        public const int MaxPhotos = 20;
        public const int ReferenceLength = 8;

        private const int MaxNameLength = 50;
        private const int MaxVersionLength = 100;
        private const int MaxColorLength = 40;
        private const int MaxDescriptionLength = 4000;
        private const int MaxMileage = 2_000_000;
        private const decimal MaxPrice = 10_000_000m;
        private const int MaxPowerKw = 2_000;
        private const int OldestYear = 1950;

        // No 0/o, 1/l/i: easy to read aloud or type over from a phone.
        private const string ReferenceAlphabet = "abcdefghjkmnpqrstuvwxyz23456789";

        private readonly List<ListingPhoto> _photos = new();

        public Guid Id { get; private set; }

        /// <summary>Short, unique code used in the public URL, e.g. /occasies/k3f9x2ab/bmw-320d-2018.</summary>
        public string Reference { get; private set; } = null!;

        public string Make { get; private set; } = null!;
        public string Model { get; private set; } = null!;
        public string? Version { get; private set; }
        public int Year { get; private set; }
        public int Mileage { get; private set; }
        public decimal Price { get; private set; }
        public FuelType FuelType { get; private set; }
        public Transmission Transmission { get; private set; }
        public int? PowerKw { get; private set; }
        public string? Color { get; private set; }
        public string? Description { get; private set; }

        public ListingStatus Status { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }
        public DateTime? PublishedAtUtc { get; private set; }
        public DateTime? SoldAtUtc { get; private set; }

        public IReadOnlyCollection<ListingPhoto> Photos => _photos.AsReadOnly();

        /// <summary>"BMW 320d Sport Line": make, model and (optional) version.</summary>
        public string Title => Version is null ? $"{Make} {Model}" : $"{Make} {Model} {Version}";

        /// <summary>Readable URL part. Not stored: it follows the details, while <see cref="Reference"/> never changes.</summary>
        public string Slug => SlugGenerator.Create($"{Title} {Year}");

        public ListingPhoto? CoverPhoto => _photos.MinBy(p => p.SortOrder);

        /// <summary>Can visitors see this listing?</summary>
        public bool IsPublic => Status is ListingStatus.ForSale or ListingStatus.Reserved or ListingStatus.Sold;

        private CarListing() { } // for EF Core

        public static CarListing Create(ListingDetails details, DateTime nowUtc)
        {
            var listing = new CarListing
            {
                Id = Guid.NewGuid(),
                Reference = GenerateReference(),
                Status = ListingStatus.Draft,
                CreatedAtUtc = nowUtc
            };

            listing.ApplyDetails(details, nowUtc);
            return listing;
        }

        public void UpdateDetails(ListingDetails details, DateTime nowUtc) => ApplyDetails(details, nowUtc);

        // ----- Status -----

        /// <summary>Put the car (back) up for sale. Needs at least one photo.</summary>
        public void Publish(DateTime nowUtc)
        {
            EnsureStatus("te koop zetten", ListingStatus.Draft, ListingStatus.Reserved);

            if (_photos.Count == 0)
                throw new DomainException("Voeg minstens één foto toe voordat u de wagen te koop zet.");

            Status = ListingStatus.ForSale;
            PublishedAtUtc ??= nowUtc;
            UpdatedAtUtc = nowUtc;
        }

        public void Reserve(DateTime nowUtc)
        {
            EnsureStatus("reserveren", ListingStatus.ForSale);
            Status = ListingStatus.Reserved;
            UpdatedAtUtc = nowUtc;
        }

        public void MarkSold(DateTime nowUtc)
        {
            EnsureStatus("als verkocht markeren", ListingStatus.ForSale, ListingStatus.Reserved);
            Status = ListingStatus.Sold;
            SoldAtUtc = nowUtc;
            UpdatedAtUtc = nowUtc;
        }

        /// <summary>Take the listing offline again (also to correct a mistake, e.g. sold by accident).</summary>
        public void Unpublish(DateTime nowUtc)
        {
            EnsureStatus("offline halen", ListingStatus.ForSale, ListingStatus.Reserved, ListingStatus.Sold);
            Status = ListingStatus.Draft;
            SoldAtUtc = null;
            UpdatedAtUtc = nowUtc;
        }

        // ----- Photos -----

        public ListingPhoto AddPhoto(int width, int height, DateTime nowUtc)
        {
            if (_photos.Count >= MaxPhotos)
                throw new DomainException($"Een occasie kan maximaal {MaxPhotos} foto's hebben.");

            var photo = new ListingPhoto(Id, _photos.Count, width, height, nowUtc);
            _photos.Add(photo);
            UpdatedAtUtc = nowUtc;
            return photo;
        }

        public void RemovePhoto(Guid photoId, DateTime nowUtc)
        {
            var photo = FindPhoto(photoId);

            if (_photos.Count == 1 && Status != ListingStatus.Draft)
                throw new DomainException("Een gepubliceerde occasie moet minstens één foto hebben. Haal ze eerst offline.");

            _photos.Remove(photo);
            Renumber(OrderedPhotos());
            UpdatedAtUtc = nowUtc;
        }

        /// <summary>Moves a photo one place to the left (-1) or right (+1) in the gallery.</summary>
        public void MovePhoto(Guid photoId, int offset, DateTime nowUtc)
        {
            var ordered = OrderedPhotos();
            var index = ordered.IndexOf(FindPhoto(photoId));
            var newIndex = Math.Clamp(index + offset, 0, ordered.Count - 1);

            if (newIndex == index)
                return;

            (ordered[index], ordered[newIndex]) = (ordered[newIndex], ordered[index]);
            Renumber(ordered);
            UpdatedAtUtc = nowUtc;
        }

        public void SetCoverPhoto(Guid photoId, DateTime nowUtc)
        {
            var photo = FindPhoto(photoId);
            var ordered = OrderedPhotos();

            ordered.Remove(photo);
            ordered.Insert(0, photo);
            Renumber(ordered);
            UpdatedAtUtc = nowUtc;
        }

        // ----- Helpers -----

        private void ApplyDetails(ListingDetails details, DateTime nowUtc)
        {
            ArgumentNullException.ThrowIfNull(details);

            // Validate everything first, so an invalid update never leaves the listing half-changed.
            var make = RequiredText(details.Make, MaxNameLength, "Merk");
            var model = RequiredText(details.Model, MaxNameLength, "Model");
            var version = OptionalText(details.Version, MaxVersionLength, "Uitvoering");
            var color = OptionalText(details.Color, MaxColorLength, "Kleur");
            var description = OptionalText(details.Description, MaxDescriptionLength, "Omschrijving");

            var latestYear = nowUtc.Year + 1; // next year's models are sold from autumn on
            if (details.Year < OldestYear || details.Year > latestYear)
                throw new DomainException($"Het bouwjaar moet tussen {OldestYear} en {latestYear} liggen.");

            if (details.Mileage is < 0 or > MaxMileage)
                throw new DomainException("Geef een geldige kilometerstand in.");

            if (details.Price is <= 0m or > MaxPrice)
                throw new DomainException("Geef een geldige prijs in.");

            if (details.PowerKw is <= 0 or > MaxPowerKw)
                throw new DomainException("Geef een geldig vermogen in (kW).");

            if (!Enum.IsDefined(details.FuelType) || !Enum.IsDefined(details.Transmission))
                throw new DomainException("Kies een geldige brandstof en versnellingsbak.");

            Make = make;
            Model = model;
            Version = version;
            Color = color;
            Description = description;
            Year = details.Year;
            Mileage = details.Mileage;
            Price = decimal.Round(details.Price, 2);
            FuelType = details.FuelType;
            Transmission = details.Transmission;
            PowerKw = details.PowerKw;
            UpdatedAtUtc = nowUtc;
        }

        private void EnsureStatus(string action, params ListingStatus[] allowed)
        {
            if (!allowed.Contains(Status))
                throw new DomainException($"Deze occasie kan nu niet {action} worden. Vernieuw de pagina en probeer opnieuw.");
        }

        private ListingPhoto FindPhoto(Guid photoId) =>
            _photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new DomainException("Deze foto bestaat niet (meer). Vernieuw de pagina.");

        private List<ListingPhoto> OrderedPhotos() => _photos.OrderBy(p => p.SortOrder).ToList();

        private static void Renumber(IReadOnlyList<ListingPhoto> ordered)
        {
            for (var i = 0; i < ordered.Count; i++)
                ordered[i].SortOrder = i;
        }

        private static string RequiredText(string? value, int maxLength, string fieldName) =>
            OptionalText(value, maxLength, fieldName)
            ?? throw new DomainException($"{fieldName} is verplicht.");

        private static string? OptionalText(string? value, int maxLength, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            var trimmed = value.Trim();

            if (trimmed.Length > maxLength)
                throw new DomainException($"{fieldName} mag maximaal {maxLength} tekens lang zijn.");

            return trimmed;
        }

        private static string GenerateReference() =>
            RandomNumberGenerator.GetString(ReferenceAlphabet, ReferenceLength);
    }
}
