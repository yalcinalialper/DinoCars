namespace DinoCars.AppLogic.Listings
{
    internal sealed class ListingCatalog(
        IListingRepository listings,
        ListingViewMapper mapper) : IListingCatalog
    {
        public async Task<IReadOnlyList<ListingCardView>> SearchAsync(ListingSearch search, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(search);

            var results = await listings.SearchPublicAsync(search, cancellationToken);
            return results.Select(mapper.ToCard).ToList();
        }

        public Task<IReadOnlyList<string>> GetMakesAsync(CancellationToken cancellationToken = default) =>
            listings.GetPublicMakesAsync(cancellationToken);

        public async Task<ListingDetailView?> FindByReferenceAsync(string reference, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return null;

            var listing = await listings.FindPublicByReferenceAsync(reference.Trim().ToLowerInvariant(), cancellationToken);
            return listing is null ? null : mapper.ToDetail(listing);
        }

        public async Task<IReadOnlyList<ListingLink>> GetPublishedLinksAsync(CancellationToken cancellationToken = default)
        {
            var published = await listings.SearchPublicAsync(new ListingSearch(), cancellationToken);
            return published.Select(listing => new ListingLink(listing.Reference, listing.Slug, listing.UpdatedAtUtc)).ToList();
        }
    }
}
