using DinoCars.AppLogic.Availability;
using DinoCars.Domain;
using DinoCars.Domain.Enums;
using DinoCars.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic.Reservations.Admin
{
    internal sealed class ReservationAdminService(
        IReservationRepository reservations,
        IUnitOfWork unitOfWork,
        IClock clock,
        IOptions<WorkshopOptions> options) : IReservationAdminService
    {
        private readonly TimeZoneInfo _timeZone = TimeZoneInfo.FindSystemTimeZoneById(options.Value.TimeZoneId);

        public async Task<IReadOnlyList<ReservationOverview>> GetPendingAsync(CancellationToken cancellationToken = default)
        {
            var pending = await reservations.GetByStatusAsync(ReservationStatus.Requested, clock.UtcNow, cancellationToken);
            return pending.Select(ToOverview).ToList();
        }

        public async Task<IReadOnlyList<ReservationOverview>> GetForDayAsync(DateOnly localDate, CancellationToken cancellationToken = default)
        {
            var fromUtc = ToUtc(localDate);
            var toUtc = ToUtc(localDate.AddDays(1));

            var dayReservations = await reservations.GetInRangeAsync(fromUtc, toUtc, cancellationToken);
            return dayReservations.Select(ToOverview).ToList();
        }

        public Task ApproveAsync(Guid reservationId, CancellationToken cancellationToken = default) =>
            ChangeStatusAsync(reservationId, r => r.Approve(), cancellationToken);

        public Task RejectAsync(Guid reservationId, CancellationToken cancellationToken = default) =>
            ChangeStatusAsync(reservationId, r => r.Reject(), cancellationToken);

        public Task CancelAsync(Guid reservationId, CancellationToken cancellationToken = default) =>
            ChangeStatusAsync(reservationId, r => r.Cancel(), cancellationToken);

        public Task CompleteAsync(Guid reservationId, CancellationToken cancellationToken = default) =>
            ChangeStatusAsync(reservationId, r => r.Complete(), cancellationToken);

        private async Task ChangeStatusAsync(Guid reservationId, Action<Reservation> change, CancellationToken cancellationToken)
        {
            var reservation = await reservations.GetByIdAsync(reservationId, cancellationToken)
                ?? throw new DomainException("Reservatie niet gevonden.");

            change(reservation);   // the domain decides whether this transition is allowed
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Later: notify the customer here (email / WhatsApp).
        }

        private ReservationOverview ToOverview(Reservation r)
        {
            var customer = r.Vehicle.Customer;

            return new ReservationOverview(
                r.Id,
                TimeZoneInfo.ConvertTimeFromUtc(r.StartUtc, _timeZone),
                TimeZoneInfo.ConvertTimeFromUtc(r.EndUtc, _timeZone),
                r.Status,
                r.Source,
                $"{customer.FirstName} {customer.LastName}",
                customer.MobilePhoneNumber,
                customer.Email,
                $"{r.Vehicle.Make} {r.Vehicle.Model}",
                r.Vehicle.LicensePlate,
                r.Mileage,
                r.Services.Select(s => s.ServiceType.Name).ToList(),
                r.CustomerNotes,
                r.AccessToken);
        }

        private DateTime ToUtc(DateOnly localDate) =>
            TimeZoneInfo.ConvertTimeToUtc(localDate.ToDateTime(TimeOnly.MinValue), _timeZone);
    }
}
