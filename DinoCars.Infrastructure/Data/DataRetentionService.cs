using DinoCars.AppLogic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DinoCars.Infrastructure.Data
{
    public sealed class DataRetentionOptions
    {
        public const string SectionName = "DataRetention";

        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Customers without any appointment in this many years are deleted, together with their
        /// vehicles and reservations (GDPR: don't keep personal data longer than needed).
        /// Must match the period mentioned in the privacy statement.
        /// </summary>
        public int RetentionYears { get; set; } = 3;
    }

    /// <summary>Runs once a day and removes customer data that is older than the retention period.</summary>
    internal sealed class DataRetentionService(
        IServiceScopeFactory scopeFactory,
        IOptions<DataRetentionOptions> options,
        ILogger<DataRetentionService> logger) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromHours(24);
        private static readonly TimeSpan StartupDelay = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!options.Value.Enabled)
                return;

            // Let the app finish starting before the first clean-up.
            await Task.Delay(StartupDelay, stoppingToken);

            using var timer = new PeriodicTimer(Interval);

            do
            {
                try
                {
                    await CleanUpAsync(stoppingToken);
                }
                catch (Exception ex) when (ex is not OperationCanceledException)
                {
                    // A failed clean-up must never crash the app; it simply tries again tomorrow.
                    logger.LogError(ex, "Data retention clean-up failed.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task CleanUpAsync(CancellationToken cancellationToken)
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();

            var retentionYears = options.Value.RetentionYears;
            var cutoffUtc = clock.UtcNow.AddYears(-retentionYears);

            // Customers that are older than the cut-off and have no appointment that ended after it.
            var staleCustomerIds = await context.Customers
                .Where(c => c.CreatedAtUtc < cutoffUtc)
                .Where(c => !context.Reservations.Any(r => r.Vehicle.CustomerId == c.Id && r.EndUtc >= cutoffUtc))
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            if (staleCustomerIds.Count == 0)
                return;

            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

            // Reservations first: they point to vehicles with "restrict", so the database won't remove them by itself.
            // Their service lines are removed automatically (cascade).
            await context.Reservations
                .Where(r => staleCustomerIds.Contains(r.Vehicle.CustomerId))
                .ExecuteDeleteAsync(cancellationToken);

            // Deleting the customer also deletes their vehicles (cascade).
            await context.Customers
                .Where(c => staleCustomerIds.Contains(c.Id))
                .ExecuteDeleteAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Data retention: deleted {Count} customer(s) without appointments in the last {Years} years.",
                staleCustomerIds.Count, retentionYears);
        }
    }
}
