namespace DinoCars.Domain.Listings
{
    /// <summary>
    /// The descriptive data of a car listing. Grouped in one record so creating and updating a listing
    /// take a single argument, and the validation rules live in one place (<see cref="CarListing"/>).
    /// </summary>
    public sealed record ListingDetails(
        string Make,
        string Model,
        string? Version,
        int Year,
        int Mileage,
        decimal Price,
        FuelType FuelType,
        Transmission Transmission,
        int? PowerKw,
        string? Color,
        string? Description);
}
