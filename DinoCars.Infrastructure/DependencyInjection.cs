using DinoCars.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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


            return services;
        }
    }
}
