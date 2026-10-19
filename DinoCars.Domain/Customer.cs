namespace DinoCars.Domain
{
    public sealed class Customer
    {
        private readonly List<Vehicle> _vehicles = new();

        public Guid Id { get; private set; }
        public string FirstName { get; private set; } = null!;
        public string LastName { get; private set; } = null!;
        public string MobilePhoneNumber { get; private set; } = null!;
        public string? Email {  get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public IReadOnlyCollection<Vehicle> Vehicles => _vehicles.AsReadOnly();

        private Customer() { }

        public Customer(string firstName, string lastName, string phone, string? email, DateTime createdAtUtc)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
            ArgumentException.ThrowIfNullOrWhiteSpace(phone);

            Id = Guid.NewGuid();
            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            MobilePhoneNumber = phone.Trim();
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim().ToLowerInvariant();
            CreatedAtUtc = createdAtUtc;
        }

        public Vehicle AddVehicle(string make, string model, string licensePlate, string vin)
        {
            var vehicle = new Vehicle(Id, make, model, licensePlate, vin);
            _vehicles.Add(vehicle);
            return vehicle;
        }
        public Vehicle? FindVehicle(string licensePlate)
        {
            var normalized = Vehicle.NormalizeLicensePlate(licensePlate);
            return _vehicles.FirstOrDefault(v => v.LicensePlate == normalized);
        }


    }
}
