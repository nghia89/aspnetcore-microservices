using Customer.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Customer.API.Controllers
{
    
    public static class CustomersController  
    {
     public static void MapCustomerController(this WebApplication app)
        {

            app.MapGet("/", () => "Welcome to customer Api");
            app.MapGet("/api/customers", (ICustomerService customerService) => customerService.GetCustomers());
            app.MapGet("/api/customer/{username}", (string username, ICustomerService customerService) => customerService.GetCustomerByUsername(username));
        }
        
    }
}