using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic.Reservations.Admin
{
    public interface IReservationAdminService
    {
        Task<IReadOnlyList<ReservationOverview>> GetPendingAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ReservationOverview>> GetForDayAsync(DateOnly localDate, CancellationToken cancellationToken = default);

        Task ApproveAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task RejectAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task CancelAsync(Guid reservationId, CancellationToken cancellationToken = default);
        Task CompleteAsync(Guid reservationId, CancellationToken cancellationToken = default);
    }
}
