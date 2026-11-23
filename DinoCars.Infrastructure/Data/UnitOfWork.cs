using DinoCars.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DinoCars.AppLogic
{
    internal sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        // How long a booking waits for another booking to finish before giving up (milliseconds).
        private const int LockTimeoutMilliseconds = 15_000;

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);

        public async Task<T> ExecuteExclusivelyAsync<T>(string lockName, Func<Task<T>> action, CancellationToken cancellationToken = default)
        {
            // The execution strategy is required by EF Core when a transaction is started manually
            // and connection retries are enabled (which is recommended on Azure SQL).
            var strategy = context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                // sp_getapplock: a named lock inside SQL Server. Because the lock belongs to the
                // transaction, it is released automatically on commit or rollback, also if the app crashes.
                // {lockName} is sent as a SQL parameter, not pasted into the SQL text.
                await context.Database.ExecuteSqlAsync($"""
                    DECLARE @result int;
                    EXEC @result = sp_getapplock @Resource = {lockName}, @LockMode = 'Exclusive', @LockOwner = 'Transaction', @LockTimeout = {LockTimeoutMilliseconds};
                    IF @result < 0 THROW 51000, 'Could not acquire the application lock.', 1;
                    """, cancellationToken);

                var result = await action();

                await transaction.CommitAsync(cancellationToken);
                return result;
            });
        }
    }
}
