namespace DinoCars.AppLogic
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Runs <paramref name="action"/> inside one database transaction while holding an exclusive,
        /// database-wide lock called <paramref name="lockName"/>. Two callers using the same lock name
        /// never run at the same time: the second one waits until the first has committed.
        /// </summary>
        Task<T> ExecuteExclusivelyAsync<T>(string lockName, Func<Task<T>> action, CancellationToken cancellationToken = default);
    }
}
