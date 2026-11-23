using DinoCars.AppLogic.Availability;
using DinoCars.Domain;
using DinoCars.Domain.Enums;
using DinoCars.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace DinoCars.AppLogic.Reservations
{
    internal sealed class ReservationBookingService(
        ICustomerRepository customers,
        IServiceTypeRepository serviceTypes,
        IReservationRepository reservations,
        IAvailabilityService availability,
        IUnitOfWork unitOfWork,
        IClock clock,
        IOptions<BookingProtectionOptions> protectionOptions) : IReservationBookingService
    {
        // Every booking (online and staff) takes this lock. Bookings therefore happen one after another,
        // never at the same moment, so two people can't grab the same slot. For a garage with a handful
        // of bookings a day, waiting a few milliseconds for each other is not noticeable.
        private const string BookingLockName = "DinoCars.Reservations.Booking";

        private readonly BookingProtectionOptions _protection = protectionOptions.Value;

        public async Task<BookingConfirmation> BookAsync(BookReservationRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Load the chosen services and check they all exist.
            var requestedIds = request.ServiceTypeIds.Distinct().ToList();
            var selectedServices = await serviceTypes.GetByIdsAsync(requestedIds, cancellationToken);

            if (selectedServices.Count != requestedIds.Count)
                throw new DomainException("Een of meer gekozen diensten bestaan niet meer. Vernieuw de pagina en probeer opnieuw.");

            var isStaffBooking = request.Source != ReservationSource.Online;
            var durationMinutes = selectedServices.Sum(s => s.DurationMinutes);

            // 2. Everything from here on runs under the booking lock, inside one transaction.
            return await unitOfWork.ExecuteExclusivelyAsync(BookingLockName, async () =>
            {
                var now = clock.UtcNow;

                // 3. Anti-spam limits, only for online bookings.
                if (!isStaffBooking)
                    await EnsureWithinLimitsAsync(request.Phone, now, cancellationToken);

                // 4. Check the slot again. Thanks to the lock, nobody else can book in between this check and the save.
                if (!await availability.IsSlotAvailableAsync(request.StartUtc, durationMinutes, isStaffBooking, cancellationToken))
                    throw new DomainException("Dit tijdstip is net door iemand anders gereserveerd. Kies een ander moment.");

                // 5. Find the returning customer by phone, or create a new one.
                var customer = await customers.FindByPhoneAsync(request.Phone, cancellationToken);

                if (customer is null)
                {
                    customer = new Customer(request.FirstName, request.LastName, request.Phone, request.Email, now);
                    customers.Add(customer);
                }

                // 6. Reuse their car if we know it, otherwise add it.
                var vehicle = customer.FindVehicle(request.LicensePlate)
                    ?? customer.AddVehicle(request.Make, request.Model, request.LicensePlate, request.Vin);

                // 7. Create the reservation. All domain rules are checked inside Create.
                var reservation = Reservation.Create(
                    vehicle, request.StartUtc, selectedServices, request.Mileage, request.CustomerNotes, now, request.Source);

                // The garage already agreed on this time with the customer, so it's confirmed immediately.
                if (isStaffBooking)
                    reservation.Approve();

                reservations.Add(reservation);

                // 8. Customer, vehicle and reservation are saved together, or nothing is.
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return new BookingConfirmation(reservation.Id, reservation.AccessToken, reservation.StartUtc, reservation.EndUtc);
            }, cancellationToken);
        }

        private async Task EnsureWithinLimitsAsync(string phone, DateTime nowUtc, CancellationToken cancellationToken)
        {
            var onlineLastHour = await reservations.CountCreatedSinceAsync(ReservationSource.Online, nowUtc.AddHours(-1), cancellationToken);

            if (onlineLastHour >= _protection.MaxOnlineBookingsPerHour)
                throw new DomainException("Er komen momenteel erg veel aanvragen binnen. Probeer het later opnieuw of bel ons even.");

            var openForPhone = await reservations.CountUpcomingForPhoneAsync(phone, nowUtc, cancellationToken);

            if (openForPhone >= _protection.MaxOpenBookingsPerPhone)
                throw new DomainException("U heeft al meerdere openstaande afspraken. Neem contact met ons op als u nog een afspraak wilt maken.");
        }
    }
}
