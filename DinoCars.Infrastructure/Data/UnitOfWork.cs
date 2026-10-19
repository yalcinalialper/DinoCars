using DinoCars.Infrastructure.Data;

namespace DinoCars.AppLogic
{
    internal sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);
    }
}
