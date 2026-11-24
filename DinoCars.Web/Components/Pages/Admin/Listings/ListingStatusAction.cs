using DinoCars.Domain.Listings;

namespace DinoCars.Web.Components.Pages.Admin.Listings
{
    public enum ListingStatusAction
    {
        Publish,
        Reserve,
        MarkSold,
        Unpublish
    }

    /// <summary>Which status buttons make sense for a listing, with their Dutch labels and confirmations.</summary>
    public static class ListingStatusActions
    {
        public static IReadOnlyList<ListingStatusAction> AvailableFor(ListingStatus status) => status switch
        {
            ListingStatus.Draft => [ListingStatusAction.Publish],
            ListingStatus.ForSale => [ListingStatusAction.Reserve, ListingStatusAction.MarkSold, ListingStatusAction.Unpublish],
            ListingStatus.Reserved => [ListingStatusAction.MarkSold, ListingStatusAction.Publish, ListingStatusAction.Unpublish],
            ListingStatus.Sold => [ListingStatusAction.Unpublish],
            _ => []
        };

        public static string Label(ListingStatusAction action, ListingStatus current) => action switch
        {
            ListingStatusAction.Publish when current == ListingStatus.Reserved => "Terug te koop zetten",
            ListingStatusAction.Publish => "Te koop zetten",
            ListingStatusAction.Reserve => "Gereserveerd",
            ListingStatusAction.MarkSold => "Verkocht",
            ListingStatusAction.Unpublish => "Offline halen",
            _ => throw new ArgumentOutOfRangeException(nameof(action))
        };

        /// <summary>The main action gets the yellow button.</summary>
        public static bool IsPrimary(ListingStatusAction action) =>
            action is ListingStatusAction.Publish or ListingStatusAction.MarkSold;

        /// <summary>A question for actions that change what customers see, or <c>null</c> when no confirmation is needed.</summary>
        public static string? Confirmation(ListingStatusAction action, string title) => action switch
        {
            ListingStatusAction.MarkSold => $"{title} als verkocht markeren?",
            ListingStatusAction.Unpublish => $"{title} offline halen? Klanten zien de wagen dan niet meer.",
            _ => null
        };
    }
}
