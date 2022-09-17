using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Contracts.Sagas.OrderManager;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestrator.OrderManager;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTOs.Basket;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Saga.Orchestrator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckoutController : ControllerBase
    {
        private readonly ISagaOrderManager<BasketCheckoutDto, OrderResponse> _sagaOrderManager;
        private readonly ICheckoutSagaService _checkOutSagaService;

        public CheckoutController(ICheckoutSagaService checkOutSagaService,
            ISagaOrderManager<BasketCheckoutDto, OrderResponse> sagaOrderManager)
        {
            _checkOutSagaService = checkOutSagaService;
            _sagaOrderManager = sagaOrderManager;
        }


        [HttpPost]
        [Route("{username}")]
        public OrderResponse CheckoutOrder([Required] string username, [FromBody] BasketCheckoutDto model)
        {
            model.UserName = username;
            var result = _sagaOrderManager.CreateOrder(model);
            return result;
        }

        //[HttpPost]
        //[Route("{username}")]
        //public Task<bool> CheckoutOrder([Required] string username,
        //    [FromBody] BasketCheckoutDto model)
        //{
        //    model.UserName = username;
        //    var result = _checkOutSagaService.CheckoutOrder(username, model);
        //    return result;
        //}
    }
}

