using DinoCars.AppLogic.Availability;
using DinoCars.Domain;
using DinoCars.Domain.Enums;
using DinoCars.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.AppLogic.Reservations
{
    internal sealed class ReservationBookingService(
        ICustomerRepository customers,
        IServiceTypeRepository serviceTypes,
        IReservationRepository reservations,
        IAvailabilityService availability,
        IUnitOfWork unitOfWork,
        IClock clock) : IReservationBookingService
    {
        public async Task<BookingConfirmation> BookAsync(BookReservationRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Load the chosen services and check they all exist.
            var requestedIds = request.ServiceTypeIds.Distinct().ToList();
            var selectedServices = await serviceTypes.GetByIdsAsync(requestedIds, cancellationToken);
            var isStaffBooking = request.Source != ReservationSource.Online;

            if (selectedServices.Count != requestedIds.Count)
                throw new DomainException("One or more selected services do not exist.");

            // 2. Check the slot again, since it may have been taken since the page loaded.
            var durationMinutes = selectedServices.Sum(s => s.DurationMinutes);

            if (!await availability.IsSlotAvailableAsync(request.StartUtc, durationMinutes, isStaffBooking, cancellationToken))
                throw new DomainException("This time slot is no longer available. Please choose another time.");

            var now = clock.UtcNow;

            // 3. Find the returning customer by phone, or create a new one.
            var customer = await customers.FindByPhoneAsync(request.Phone, cancellationToken);

            if (customer is null)
            {
                customer = new Customer(request.FirstName, request.LastName, request.Phone, request.Email, now);
                customers.Add(customer);
            }

            // 4. Reuse their car if we know it, otherwise add it.
            var vehicle = customer.FindVehicle(request.LicensePlate)
                ?? customer.AddVehicle(request.Make, request.Model, request.LicensePlate, request.Vin);

            // creating the reservation:
            var reservation = Reservation.Create(
                vehicle, request.StartUtc, selectedServices, request.Mileage, request.CustomerNotes, now, request.Source);

            // The garage already agreed on this time with the customer, so it's confirmed immediately.
            if (isStaffBooking)
                reservation.Approve();

            reservations.Add(reservation);

            // 6. One transaction: customer, vehicle and reservation are saved together, or nothing is.
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new BookingConfirmation(reservation.Id, reservation.AccessToken, reservation.StartUtc, reservation.EndUtc);
        }
    }
}
