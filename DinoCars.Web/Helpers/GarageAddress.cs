namespace DinoCars.Web.Helpers
{
    /// <summary>The garage address as structured data (schema.org PostalAddress), shared by all JSON-LD blocks.</summary>
    public static class GarageAddress
    {
        public static Dictionary<string, object> AsSchema() => new()
        {
            ["@type"] = "PostalAddress",
            ["streetAddress"] = GarageInfo.Street,
            ["postalCode"] = GarageInfo.PostalCode,
            ["addressLocality"] = GarageInfo.City,
            ["addressCountry"] = "BE"
        };
    }
}
