using DinoCars.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic.Reservations.Admin
{
    public sealed record ReservationOverview(
        Guid Id,
        DateTime StartLocal,
        DateTime EndLocal,
        ReservationStatus Status,
        ReservationSource Source,
        string CustomerName,
        string Phone,
        string? Email,
        string Vehicle,
        string LicensePlate,
        int Mileage,
        IReadOnlyList<string> Services,
        string? Notes,
        string AccessToken);
}
