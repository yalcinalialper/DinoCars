using DinoCars.AppLogic;
using DinoCars.Domain;
using DinoCars.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DinoCars.Infrastructure.Repositories
{
    internal sealed class ServiceTypeRepository(ApplicationDbContext context) : IServiceTypeRepository
    {
        public async Task<IReadOnlyList<ServiceType>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await context.ServiceTypes
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<ServiceType>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids.Distinct().ToList();

            return await context.ServiceTypes
                .Where(s => idList.Contains(s.Id))
                .ToListAsync(cancellationToken);
        }
    }
}
