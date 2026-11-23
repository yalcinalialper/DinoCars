using DinoCars.Domain.Enums;
using DinoCars.Domain.Exceptions;
using System.Security.Cryptography;

namespace DinoCars.Domain
{
    public sealed class Reservation
    {
        private const int MaxNotesLength = 1000;
        private readonly List<ReservationService> _services = new();

        public Guid Id { get; private set; }
        public Guid VehicleId { get; private set; }
        public Vehicle Vehicle { get; private set; } = null!;
        public DateTime StartUtc { get; private set;  }
        public DateTime EndUtc { get; private set; }

        public ReservationStatus Status { get; private set;  }
        public ReservationSource Source { get; private set; }
        public int Mileage { get; private set; }
        public string? CustomerNotes { get; private set; }
        public string AccessToken { get; private set; } = null!;
        public DateTime CreatedAtUtc { get; private set; }

        public IReadOnlyCollection<ReservationService> Services => _services.AsReadOnly();

        private Reservation() { }

        public static Reservation Create(
            Vehicle vehicle,
            DateTime startUtc,
            IReadOnlyCollection<ServiceType> serviceTypes,
            int mileage,
            string? customerNotes,
            DateTime nowUtc,
            ReservationSource source = ReservationSource.Online)
        {
            ArgumentNullException.ThrowIfNull(vehicle, nameof(vehicle));
            ArgumentNullException.ThrowIfNull(serviceTypes, nameof(serviceTypes));

            if (serviceTypes.Count == 0)
                throw new DomainException("Kies minstens één dienst.");

            if (serviceTypes.Any(s => !s.IsActive))
                throw new DomainException("Een van de gekozen diensten is niet meer beschikbaar.");

            if (startUtc <= nowUtc)
                throw new DomainException("Een afspraak moet in de toekomst liggen.");

            if (mileage < 0)
                throw new DomainException("De kilometerstand kan niet negatief zijn.");

            var notes = string.IsNullOrWhiteSpace(customerNotes) ? null : customerNotes.Trim();

            if (serviceTypes.Any(s => s.Code == ServiceType.OtherCode) && notes is null)
                throw new DomainException("Omschrijf het probleem wanneer u 'Overige' kiest.");

            if (notes?.Length > MaxNotesLength)
                throw new DomainException($"Opmerkingen mogen maximaal {MaxNotesLength} tekens lang zijn.");

            var reservation = new Reservation
            {
                Id = Guid.NewGuid(),
                VehicleId = vehicle.Id,
                Vehicle = vehicle,
                StartUtc = startUtc,
                Status = ReservationStatus.Requested,
                Mileage = mileage,
                CustomerNotes = notes,
                AccessToken = GenerateAccessToken(),
                CreatedAtUtc = nowUtc,
                Source = source
            };

            foreach (var serviceType in serviceTypes.DistinctBy(s => s.Id))
                reservation._services.Add(new ReservationService(reservation.Id, serviceType));

            reservation.EndUtc = startUtc.AddMinutes(reservation._services.Sum(s => s.DurationMinutes));
            return reservation;

        }

        public void Approve() => TransitionTo(ReservationStatus.Approved, from: ReservationStatus.Requested);

        public void Reject() => TransitionTo(ReservationStatus.Rejected, from: ReservationStatus.Requested);

        public void Complete() => TransitionTo(ReservationStatus.Completed, from: ReservationStatus.Approved);

        public void Cancel() =>
            TransitionTo(ReservationStatus.Cancelled, ReservationStatus.Requested, ReservationStatus.Approved);

        public static readonly IReadOnlyCollection<ReservationStatus> CapacityBlockingStatuses =
            [ReservationStatus.Requested, ReservationStatus.Approved, ReservationStatus.Completed];

        /// <summary>Does this reservation take up workshop capacity?</summary>
        public bool BlocksCapacity => CapacityBlockingStatuses.Contains(Status);

        public bool Overlaps(DateTime startUtc, DateTime endUtc) =>
            startUtc < EndUtc && endUtc > StartUtc;

        private void TransitionTo(ReservationStatus target, params ReservationStatus[] from)
        {
            if (!from.Contains(Status))
                throw new DomainException("Deze afspraak is intussen al gewijzigd. Vernieuw de pagina en probeer opnieuw.");

            Status = target;
        }

        private static string GenerateAccessToken() =>
            Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
    }
}
