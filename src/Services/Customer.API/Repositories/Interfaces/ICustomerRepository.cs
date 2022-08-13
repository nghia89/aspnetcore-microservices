using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;
using Customer.API.Persistence;

namespace Customer.API.Repositories.Interfaces
{
    public interface ICustomerRepository : IRepositoryQueryBase<Entities.Customer, int, CustomerContext>
    {
        Task<Entities.Customer> GetCustomerByUerName(string userName);
        Task<List<Entities.Customer>> GetCustomers();
    }
}
