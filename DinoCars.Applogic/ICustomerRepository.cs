using DinoCars.Domain;

namespace DinoCars.AppLogic
{
    public interface ICustomerRepository
    {
        Task<Customer?> FindByPhoneAsync(string phone, CancellationToken cancellationToken = default);
        void Add(Customer customer);
    }
}
