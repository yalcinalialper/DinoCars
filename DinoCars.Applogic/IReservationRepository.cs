using DinoCars.Domain;
using DinoCars.Domain.Enums;

namespace DinoCars.AppLogic
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Reservation?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Reservation>> GetBlockingInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);

        /// <summary>All reservations overlapping the period, any status, with customer, vehicle and services.</summary>
        Task<IReadOnlyList<Reservation>> GetInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Reservation>> GetByStatusAsync(ReservationStatus status, DateTime fromUtc, CancellationToken cancellationToken = default);

        /// <summary>Requested or approved reservations of the customer with this phone number that haven't ended yet.</summary>
        Task<int> CountUpcomingForPhoneAsync(string phone, DateTime fromUtc, CancellationToken cancellationToken = default);

        /// <summary>Reservations from the given source that were created since <paramref name="sinceUtc"/>.</summary>
        Task<int> CountCreatedSinceAsync(ReservationSource source, DateTime sinceUtc, CancellationToken cancellationToken = default);

        void Add(Reservation reservation);
    }
}
