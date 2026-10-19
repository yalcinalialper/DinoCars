using DinoCars.AppLogic;
using DinoCars.Domain;
using DinoCars.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DinoCars.Infrastructure.Repositories
{
    internal sealed class CustomerRepository(ApplicationDbContext context) : ICustomerRepository
    {
        public void Add(Customer customer)
        {
            context.Customers.Add(customer);
        }

        public Task<Customer?> FindByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        {
            var normalized = phone.Trim();

            return context.Customers
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.MobilePhoneNumber == normalized, cancellationToken);
        }
    }
}
