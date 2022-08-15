using Ordering.Domain.Entities;
using Contracts.Common.Interfaces;
using Contracts.Domains.Interfaces;

namespace Ordering.Application.Common.Interfaces
{
    public interface IOrderRepository : IRepositoryBaseAsync<Order, long>
    {
        Task<IEnumerable<Order>> GetOrdersByUserName(string userName);
        Order CreateOrder(Order model);


        Task<Order> UpdateOrderAsync(Order order);
        void DeleteOrder(Order order);
    }
}
