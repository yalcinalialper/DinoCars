using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic.Reservations
{
    public sealed record BookingConfirmation(Guid ReservationId, string AccessToken, DateTime StartUtc, DateTime EndUtc);
}
