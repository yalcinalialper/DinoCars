namespace DinoCars.AppLogic.Listings
{
    /// <summary>Where the files of a listing photo are stored. One place, so saving and deleting always agree.</summary>
    internal static class ListingPhotoFiles
    {
        public static string Large(Guid listingId, Guid photoId) => $"listings/{listingId:N}/{photoId:N}.jpg";

        public static string Thumbnail(Guid listingId, Guid photoId) => $"listings/{listingId:N}/{photoId:N}-thumb.jpg";

        public static IEnumerable<string> All(Guid listingId, Guid photoId) =>
            [Large(listingId, photoId), Thumbnail(listingId, photoId)];
    }
}
