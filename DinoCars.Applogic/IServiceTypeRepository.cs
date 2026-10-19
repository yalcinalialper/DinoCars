using DinoCars.Domain;

namespace DinoCars.AppLogic
{
    public interface IServiceTypeRepository
    {
        Task<IReadOnlyList<ServiceType>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ServiceType>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    }
}
