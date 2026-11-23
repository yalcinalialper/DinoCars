using DinoCars.AppLogic;
using DinoCars.Domain;
using DinoCars.Domain.Enums;
using DinoCars.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DinoCars.Infrastructure.Repositories
{
    internal sealed class ReservationRepository(ApplicationDbContext context) : IReservationRepository
    {
        private IQueryable<Reservation> WithDetails =>
            context.Reservations
                .Include(r => r.Vehicle).ThenInclude(v => v.Customer)
                .Include(r => r.Services).ThenInclude(s => s.ServiceType);

        public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            WithDetails.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        public Task<Reservation?> GetByAccessTokenAsync(string accessToken, CancellationToken cancellationToken = default) =>
            WithDetails.FirstOrDefaultAsync(r => r.AccessToken == accessToken, cancellationToken);

        public async Task<IReadOnlyList<Reservation>> GetBlockingInRangeAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default) =>
            await context.Reservations
                .AsNoTracking()
                .Where(r => Reservation.CapacityBlockingStatuses.Contains(r.Status))
                .Where(r => r.StartUtc < toUtc && r.EndUtc > fromUtc)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Reservation>> GetInRangeAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default) =>
            await WithDetails
                .AsNoTracking()
                .Where(r => r.StartUtc < toUtc && r.EndUtc > fromUtc)
                .OrderBy(r => r.StartUtc)
                .ToListAsync(cancellationToken);

        public async Task<IReadOnlyList<Reservation>> GetByStatusAsync(
            ReservationStatus status, DateTime fromUtc, CancellationToken cancellationToken = default) =>
            await WithDetails
                .AsNoTracking()
                .Where(r => r.Status == status && r.EndUtc >= fromUtc)
                .OrderBy(r => r.StartUtc)
                .ToListAsync(cancellationToken);

        public Task<int> CountUpcomingForPhoneAsync(string phone, DateTime fromUtc, CancellationToken cancellationToken = default)
        {
            var normalized = phone.Trim();

            return context.Reservations.CountAsync(r =>
                r.Vehicle.Customer.MobilePhoneNumber == normalized &&
                r.EndUtc > fromUtc &&
                (r.Status == ReservationStatus.Requested || r.Status == ReservationStatus.Approved),
                cancellationToken);
        }

        public Task<int> CountCreatedSinceAsync(ReservationSource source, DateTime sinceUtc, CancellationToken cancellationToken = default) =>
            context.Reservations.CountAsync(r => r.Source == source && r.CreatedAtUtc >= sinceUtc, cancellationToken);

        public void Add(Reservation reservation) => context.Reservations.Add(reservation);
    }
}
