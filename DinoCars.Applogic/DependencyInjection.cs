using DinoCars.AppLogic.Availability;
using DinoCars.AppLogic.Reservations;
using Microsoft.Extensions.DependencyInjection;

namespace DinoCars.AppLogic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplogic(this IServiceCollection services)
        {
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            services.AddScoped<IReservationBookingService, ReservationBookingService>();
            return services;
        }
    }
}
