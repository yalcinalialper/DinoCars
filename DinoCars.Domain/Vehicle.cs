namespace DinoCars.Domain
{
    public sealed class Vehicle
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public string Make { get; private set; } = null!;
        public string Model { get; private set; } = null!;
        public string LicensePlate { get; private set; } = null!;
        public string? Vin { get; private set; }

        private Vehicle() { }

        internal Vehicle(Guid customerId, string make, string model, string licensePlate, string? vin)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(make);
            ArgumentException.ThrowIfNullOrWhiteSpace(model);
            ArgumentException.ThrowIfNullOrWhiteSpace(licensePlate);

            Id = Guid.NewGuid();
            CustomerId = customerId;
            Make = make.Trim();
            Model = model.Trim();
            LicensePlate = licensePlate.Trim().ToUpperInvariant();
            Vin = NormalizeVin(vin);
        }

        private static string? NormalizeVin(string? vin)
        {
            if (string.IsNullOrWhiteSpace(vin)) return null;

            var normalized = vin.Replace(" ", "").ToUpperInvariant();

            if (normalized.Length != 17 || normalized.Any(c => c is 'I' or 'O' or 'Q') || !normalized.All(char.IsLetterOrDigit))
                throw new ArgumentException("A VIN must be 17 letters/digits and cannot contain I, O or Q.", nameof(vin));

            return normalized;
        }
    }
}