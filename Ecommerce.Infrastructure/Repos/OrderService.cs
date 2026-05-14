using Ecommerce.Core.Dtos.Enums;
using Ecommerce.Core.Dtos.Order;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Infrastructure.Repos
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderDto>> GetMyOrdersAsync(string userId)
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.PlacedAt)
                .Select(o => ToDto(o))
                .ToListAsync();
            return orders;
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);
            return order is null ? null : ToDto(order);
        }

        public async Task<(OrderDto? Order, List<string> Errors)> PlaceOrderAsync(string userId, PlaceOrderDto dto)
        {
            var errors = new List<string>();

            var productIds = dto.Items.Select(i => i.ProductId).ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id) && p.IsActive)
                .ToListAsync();

            foreach (var item in dto.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product is null)
                    errors.Add($"Product {item.ProductId} not found.");
                else if (product.Stock < 1)
                    errors.Add($"'{product.Name}' is out of stock.");
                else if (product.Stock < item.Quantity)
                    errors.Add($"'{product.Name}' only has {product.Stock} units in stock.");
            }

            if (errors.Count > 0)
                return (null, errors);

            var orderItems = dto.Items.Select(item =>
            {
                var product = products.First(p => p.Id == item.ProductId);
                product.Stock -= item.Quantity;
                return new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
            }).ToList();

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = dto.ShippingAddress,
                Status = OrderStatus.Pending,
                PlacedAt = DateTime.UtcNow,
                Total = orderItems.Sum(i => i.UnitPrice * i.Quantity),
                OrderItems = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return (ToDto(order), errors);
        }

        public async Task<bool> UpdateStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is null) 
                return false;

            order.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        private static OrderDto ToDto(Order o) => new(
        Id: o.Id,
        UserId: o.UserId,
        Status: o.Status,
        Total: o.Total,
        ShippingAddress: o.ShippingAddress,
        PlacedAt: o.PlacedAt,
        Items: o.OrderItems.Select(oi => new OrderItemDto(
            ProductId: oi.ProductId,
            ProductName: oi.Product.Name,
            Quantity: oi.Quantity,
            UnitPrice: oi.UnitPrice,
            LineTotal: oi.LineTotal
        )).ToList()
    );
    }
}
