using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;

namespace Customer.API.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<IResult> GetCustomerByUsername(string username) => Results.Ok(await _customerRepository.GetCustomerByUerName(username));

        public async Task<IResult> GetCustomers() =>  Results.Ok(await _customerRepository.GetCustomers());
    }
}
