using System;
using Ordering.Application.Common.Models;
using Shared.DTOs.Order;
using OrderDto = Shared.DTOs.Order.OrderDto;

namespace Saga.Orchestrator.HttpRepository.Interfaces
{
    public interface IOrderHttpRepository
    {
        Task<long> CreateOrder(CreateOrderDto order);
        Task<OrderDto> GetOrder(long id);
        Task<bool> DeleteOrder(long id);
        Task<bool> DeleteOrderByDocumentNo(string documentNo);
    }
}

