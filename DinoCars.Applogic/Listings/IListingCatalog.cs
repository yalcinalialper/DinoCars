namespace DinoCars.AppLogic.Listings
{
    /// <summary>Read-only access to the cars for sale, for the public website.</summary>
    public interface IListingCatalog
    {
        Task<IReadOnlyList<ListingCardView>> SearchAsync(ListingSearch search, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<string>> GetMakesAsync(CancellationToken cancellationToken = default);

        /// <summary>The public detail of a car, or null if it doesn't exist or isn't published.</summary>
        Task<ListingDetailView?> FindByReferenceAsync(string reference, CancellationToken cancellationToken = default);

        /// <summary>Links to every published car, for the sitemap.</summary>
        Task<IReadOnlyList<ListingLink>> GetPublishedLinksAsync(CancellationToken cancellationToken = default);
    }
}
