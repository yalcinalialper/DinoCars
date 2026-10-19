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

        void Add(Reservation reservation);
    }
}
