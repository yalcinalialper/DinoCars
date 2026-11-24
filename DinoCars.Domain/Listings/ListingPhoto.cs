namespace DinoCars.Domain.Listings
{
    /// <summary>
    /// A photo of a car listing. The domain only knows its identity and order;
    /// where the image file is stored is decided by the application layer.
    /// </summary>
    public sealed class ListingPhoto
    {
        public Guid Id { get; private set; }
        public Guid ListingId { get; private set; }

        /// <summary>Position in the gallery. 0 is the cover photo.</summary>
        public int SortOrder { get; internal set; }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private ListingPhoto() { } // for EF Core

        internal ListingPhoto(Guid listingId, int sortOrder, int width, int height, DateTime createdAtUtc)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(width);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(height);

            Id = Guid.NewGuid();
            ListingId = listingId;
            SortOrder = sortOrder;
            Width = width;
            Height = height;
            CreatedAtUtc = createdAtUtc;
        }
    }
}
