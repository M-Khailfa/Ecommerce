using Ecommerce.Core.Dtos.Enums;
using Ecommerce.Core.Dtos.Order;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetMyOrdersAsync(string userId);
        Task<OrderDto?> GetByIdAsync(int id);
        Task<(OrderDto? Order, List<string> Errors)> PlaceOrderAsync(string userId, PlaceOrderDto dto);
        Task<bool> UpdateStatusAsync(int id, OrderStatus status);
    }
}
