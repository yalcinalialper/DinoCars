using DinoCars.AppLogic.Availability;
using DinoCars.AppLogic.Listings;
using DinoCars.AppLogic.Listings.Admin;
using DinoCars.AppLogic.Reservations;
using DinoCars.AppLogic.Reservations.Admin;
using Microsoft.Extensions.DependencyInjection;

namespace DinoCars.AppLogic
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplogic(this IServiceCollection services)
        {
            services.AddScoped<IAvailabilityService, AvailabilityService>();
            services.AddScoped<IReservationBookingService, ReservationBookingService>();
            services.AddScoped<IReservationAdminService, ReservationAdminService>();

            // Car listings (occasies)
            services.AddScoped<ListingViewMapper>();
            services.AddScoped<IListingCatalog, ListingCatalog>();
            services.AddScoped<IListingAdminService, ListingAdminService>();
            return services;
        }
    }
}
