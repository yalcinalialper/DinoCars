using DinoCars.Domain.Listings;

namespace DinoCars.AppLogic.Listings
{
    public interface IListingRepository
    {
        /// <summary>A listing with its photos, tracked for changes (for the back office).</summary>
        Task<CarListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>A listing visitors may see (for sale, reserved or sold), read-only.</summary>
        Task<CarListing?> FindPublicByReferenceAsync(string reference, CancellationToken cancellationToken = default);

        /// <summary>Listings shown in the public overview (for sale and reserved), filtered and sorted, read-only.</summary>
        Task<IReadOnlyList<CarListing>> SearchPublicAsync(ListingSearch search, CancellationToken cancellationToken = default);

        /// <summary>The makes that currently appear in the public overview, for the filter dropdown.</summary>
        Task<IReadOnlyList<string>> GetPublicMakesAsync(CancellationToken cancellationToken = default);

        /// <summary>All listings in any status, newest change first, read-only (for the back office).</summary>
        Task<IReadOnlyList<CarListing>> GetAllAsync(CancellationToken cancellationToken = default);

        void Add(CarListing listing);
        void Remove(CarListing listing);
    }
}
