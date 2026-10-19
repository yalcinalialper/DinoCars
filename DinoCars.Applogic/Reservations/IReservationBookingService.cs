using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic.Reservations
{
    public interface IReservationBookingService
    {
        Task<BookingConfirmation> BookAsync(BookReservationRequest request, CancellationToken cancellationToken = default);
    }
}
