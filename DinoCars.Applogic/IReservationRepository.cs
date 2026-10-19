using DinoCars.Domain;

namespace DinoCars.AppLogic
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Reservation?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Reservation>> GetBlockingInRangeAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Reservation>> GetUpcomingAsync(DateTime fromUtc, CancellationToken cancellationToken = default);

        void Add(Reservation reservation);
    }
}
