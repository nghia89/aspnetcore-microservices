namespace Customer.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IResult> GetCustomerByUsername(string username);
        Task<IResult> GetCustomers();
    }
}
