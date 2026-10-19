namespace DinoCars.AppLogic
{
    internal interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
