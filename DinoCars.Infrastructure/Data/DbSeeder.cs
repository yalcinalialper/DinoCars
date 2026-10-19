using DinoCars.Domain;
using Microsoft.EntityFrameworkCore;

namespace DinoCars.Infrastructure.Data
{
    public static class DbSeeder
    {
        private static readonly (string Code, string Name, int DurationMinutes)[] DefaultServiceTypes =
[
    ("MAINTENANCE", "Onderhoud", 60),
        ("LIGHTS_CHECK", "Lichten checken", 60),
        ("BRAKES_CHECK", "Remmen controleren", 60),
        ("BATTERY_TEST", "12V-accu testen", 60),
        (ServiceType.OtherCode, "Overige", 60)
];

        public static void Seed(DbContext context)
        {
            var serviceTypes = context.Set<ServiceType>();
            var existingCodes = serviceTypes.Select(s => s.Code).ToHashSet();

            foreach (var (code, name, duration) in DefaultServiceTypes)
            {
                if (!existingCodes.Contains(code))
                    serviceTypes.Add(new ServiceType(code, name, duration));
            }

            context.SaveChanges();
        }

        public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
        {
            var serviceTypes = context.Set<ServiceType>();
            var existingCodes = (await serviceTypes.Select(s => s.Code).ToListAsync(cancellationToken)).ToHashSet();

            foreach (var (code, name, duration) in DefaultServiceTypes)
            {
                if (!existingCodes.Contains(code))
                    serviceTypes.Add(new ServiceType(code, name, duration));
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
