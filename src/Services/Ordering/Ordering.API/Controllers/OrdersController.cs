using AutoMapper;
using Contracts.Messages;
using Contracts.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Common.Features.V1.Orders;
using Ordering.Application.Common.Interfaces;
using Ordering.Application.Common.Models;
using Ordering.Domain.Entities;
using Shared.Services;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Ordering.API;

[Route("api/v1/[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ISmtpEmailService _smtpEmailService;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly IMessageProducer _messageProducer;
    public OrdersController(IMediator mediator, ISmtpEmailService smtpEmailService, IOrderRepository orderRepository, IMapper mapper, IMessageProducer messageProducer = null)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _smtpEmailService = smtpEmailService ?? throw new ArgumentNullException(nameof(smtpEmailService));
        _orderRepository = orderRepository;
        _mapper = mapper;
        _messageProducer = messageProducer;
    }

    private static class RouteNames
    {
        public const string GetOrders = nameof(GetOrders);
    }

    [HttpGet("{username}", Name = RouteNames.GetOrders)]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByUserName([Required] string username)
    {
        var query = new GetOrdersQuery(username);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("test-email")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task TestSendEmail()
    {
        var from = new MailRequest
        {
            Body = "<h1>hello<h/1>",
            Subject = "test",
            ToAddress = "hangnghia11089@gmail.com"
        };
        await _smtpEmailService.SendEmailAsync(from);
    }

    [HttpPost("add")]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> Add([FromBody] OrderDto model)
    {
        var order = _mapper.Map<Order>(model);
        var entity = await _orderRepository.CreateOrder(order);
        await _orderRepository.SaveChangesAsync();

        _messageProducer.SendMessage(entity);
        return Ok(_mapper.Map<OrderDto>(entity));
    }
}