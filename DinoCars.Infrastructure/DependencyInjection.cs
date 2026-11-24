using DinoCars.AppLogic;
using DinoCars.AppLogic.Listings;
using DinoCars.Infrastructure.Data;
using DinoCars.Infrastructure.Media;
using DinoCars.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DinoCars.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options => options
            .UseSqlServer(connectionString)
            .UseSeeding((context, _) => DbSeeder.Seed(context))
            .UseAsyncSeeding((context, _, cancellationToken) => DbSeeder.SeedAsync(context, cancellationToken)));
            services.AddSingleton<IClock, SystemClock>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();

            // Car listings (occasies) and their photos.
            services.AddScoped<IListingRepository, ListingRepository>();
            services.Configure<MediaOptions>(configuration.GetSection(MediaOptions.SectionName));
            services.AddSingleton<IImageStorage, LocalImageStorage>();
            services.AddSingleton<IImageProcessor, ImageSharpImageProcessor>();

            // GDPR: daily clean-up of customer data older than the retention period.
            services.Configure<DataRetentionOptions>(configuration.GetSection(DataRetentionOptions.SectionName));
            services.AddHostedService<DataRetentionService>();


            return services;
        }
    }
}
