namespace DinoCars.Domain.Listings
{
    /// <summary>Life cycle of a car listing: Draft → ForSale ⇄ Reserved → Sold.</summary>
    public enum ListingStatus
    {
        /// <summary>Being prepared; not visible to the public.</summary>
        Draft = 0,

        /// <summary>Published and for sale.</summary>
        ForSale = 1,

        /// <summary>Still visible, but a buyer has reserved it.</summary>
        Reserved = 2,

        /// <summary>Sold. Hidden from the overview; the detail page shows it as sold.</summary>
        Sold = 3
    }
}
