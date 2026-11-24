using DinoCars.AppLogic.Listings;
using DinoCars.Domain.Listings;
using DinoCars.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DinoCars.Infrastructure.Repositories
{
    internal sealed class ListingRepository(ApplicationDbContext context) : IListingRepository
    {
        // Shown in the overview.
        private static readonly ListingStatus[] OverviewStatuses = [ListingStatus.ForSale, ListingStatus.Reserved];

        // Reachable by link (a sold car's page still opens, marked as sold).
        private static readonly ListingStatus[] PublicStatuses = [ListingStatus.ForSale, ListingStatus.Reserved, ListingStatus.Sold];

        private IQueryable<CarListing> ReadOnlyWithPhotos =>
            context.CarListings.AsNoTracking().Include(l => l.Photos);

        public Task<CarListing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            context.CarListings
                .Include(l => l.Photos)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

        public Task<CarListing?> FindPublicByReferenceAsync(string reference, CancellationToken cancellationToken = default) =>
            ReadOnlyWithPhotos.FirstOrDefaultAsync(
                l => l.Reference == reference && PublicStatuses.Contains(l.Status),
                cancellationToken);

        public async Task<IReadOnlyList<CarListing>> SearchPublicAsync(ListingSearch search, CancellationToken cancellationToken = default)
        {
            var query = ReadOnlyWithPhotos.Where(l => OverviewStatuses.Contains(l.Status));

            if (!string.IsNullOrWhiteSpace(search.Make))
                query = query.Where(l => l.Make == search.Make);

            if (search.FuelType is { } fuelType)
                query = query.Where(l => l.FuelType == fuelType);

            if (search.MaxPrice is { } maxPrice)
                query = query.Where(l => l.Price <= maxPrice);

            query = search.Sort switch
            {
                ListingSort.PriceLowToHigh => query.OrderBy(l => l.Price),
                ListingSort.PriceHighToLow => query.OrderByDescending(l => l.Price),
                ListingSort.MileageLowToHigh => query.OrderBy(l => l.Mileage),
                _ => query.OrderByDescending(l => l.PublishedAtUtc)
            };

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<string>> GetPublicMakesAsync(CancellationToken cancellationToken = default) =>
            await context.CarListings
                .Where(l => OverviewStatuses.Contains(l.Status))
                .Select(l => l.Make)
                .Distinct()
                .OrderBy(make => make)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<CarListing>> GetAllAsync(CancellationToken cancellationToken = default) =>
            await ReadOnlyWithPhotos
                .OrderByDescending(l => l.UpdatedAtUtc)
                .ToListAsync(cancellationToken);

        public void Add(CarListing listing) => context.CarListings.Add(listing);

        public void Remove(CarListing listing) => context.CarListings.Remove(listing);
    }
}
