using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories
{
    public class CustomerRepository : RepositoryBaseAsync<Entities.Customer, int, CustomerContext>, ICustomerRepository
    {
        public CustomerRepository(CustomerContext dbContext, IUnitOfWork<CustomerContext> unitOfWork) : base(dbContext, unitOfWork)
        {
        }

        public  Task<Entities.Customer> GetCustomerByUerName(string userName) =>  FindByCondition(x => x.UserName.Equals(userName)).SingleOrDefaultAsync();

        public async Task<List<Entities.Customer>> GetCustomers() =>await FindAll().ToListAsync();
    }
}
