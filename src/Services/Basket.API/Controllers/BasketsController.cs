using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcStockServices;
using Basket.API.Repositories.Interfaces;
using Basket.API.Services.Interfaces;
using EventBus.Messages.IntegrationEvents.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Shared.DTOs.Basket;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Basket.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class BasketsController : Controller
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly StockItemGrpcService _stockItemGrpcService;
        private readonly IEmailTemplateService _emailTemplateService;
        public BasketsController(IBasketRepository basketRepository, IMapper mapper, IPublishEndpoint publishEndpoint, StockItemGrpcService stockItemGrpcService, IEmailTemplateService emailTemplateService)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
            _stockItemGrpcService = stockItemGrpcService;
            _emailTemplateService = emailTemplateService;
        }

        [HttpGet("{username}", Name = "GetBasket")]
        [ProducesResponseType(typeof(Cart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Cart>> GetBasket([Required] string username)
        {
            var result = await _basketRepository.GetBasketByUserName(username);

            return Ok(result ?? new Cart(username));
        }

        [HttpPost(Name = "UpdateBasket")]
        [ProducesResponseType(typeof(CartDto), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CartDto>> UpdateBasket([FromBody] CartDto model)
        {
            // Communicate with Inventory.Product.Grpc and check quantity available of products
            foreach (var item in model.Items)
            {
                var stock = await _stockItemGrpcService.GetStock(item.ItemNo);
                item.SetAvailableQuantity(stock.Quantity);
            }

            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.UtcNow.AddHours(10));
            //     .SetSlidingExpiration(TimeSpan.FromMinutes(10));
            var cart = _mapper.Map<Cart>(model);
            var updatedCart = await _basketRepository.UpdateBasket(cart, options);
            var result = _mapper.Map<CartDto>(updatedCart);
            return Ok(result);
        }

        [HttpDelete("{username}", Name = "DeleteBasket")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<bool>> DeleteBasket([Required] string username)
        {
            var result = await _basketRepository.DeleteBasketFromUserName(username);
            return Ok(result);
        }

        [Route("[action]/{username}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([Required] string username, [FromBody] BasketCheckout basketCheckout)
        {
            var basket = await _basketRepository.GetBasketByUserName(username);
            if (basket == null || !basket.Items.Any()) return NotFound();

            //publish checkout event to EventBus Message
            var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);
            //remove the basket
            //await _basketRepository.DeleteBasketFromUserName(basket.Username);

            return Accepted();
        }

        [HttpPost("[action]", Name = "SenEmail")]
        public ContentResult SenEmail()
        {
            var email = _emailTemplateService.GenerateReminderCheckoutOrderEmail("customer");
            return new ContentResult
            {
                Content = email,
                ContentType = "text/html"
            };
        }
    }
}
