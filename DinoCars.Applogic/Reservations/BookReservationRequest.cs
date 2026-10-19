using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic.Reservations
{
    public sealed record BookReservationRequest(
        string FirstName,
        string LastName,
        string Phone,
        string? Email,
        string Make,
        string Model,
        string LicensePlate,
        string? Vin,
        int Mileage,
        IReadOnlyCollection<Guid> ServiceTypeIds,
        DateTime StartUtc,
        string? CustomerNotes);
}
