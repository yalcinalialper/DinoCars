namespace DinoCars.Domain
{
    public sealed class ServiceType
    {
        public const string OtherCode = "OTHER";
        public Guid Id { get; private set; }
        public string Code { get; private set; } = null!;
        public string Name { get; private set; } = null!;
        public int DurationMinutes { get; private set; }
        public bool IsActive { get; private set; }

        private ServiceType() { }

        public ServiceType(string code, string name, int durationMinutes)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(durationMinutes);

            Id = Guid.NewGuid();
            Code = code.Trim().ToUpperInvariant();
            Name = name.Trim();
            DurationMinutes = durationMinutes;
            IsActive = true;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;
    }
}
