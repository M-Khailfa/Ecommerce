using Ecommerce.Core.Dtos.Enums;
using Ecommerce.Core.Dtos.Payment;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Infrastructure.Repos
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly Paymob _paymob;
        private readonly IPaymobService _paymobService;
        public PaymentService(AppDbContext context, Paymob paymob)
        {
            _context = context;
            _paymob = paymob;
        }

        public async Task<PaymentDto?> GetByOrderAsync(int orderId)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
            return payment is null ? null : ToDto(payment);
        }

        //public async Task<(PaymentDto? Payment, string? Error)> PayAsync(string userId, CreatePaymentDto dto)
        //{
        //    var order = await _context.Orders
        //        .Include(o => o.Payment)
        //        .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

        //    if (order is null)
        //        return (null, "Order not found.");

        //    if (order.UserId != userId)
        //        return (null, "Forbidden.");

        //    if (order.Payment is not null)
        //        return (null, "This order has already been paid.");

        //    if (order.Status == OrderStatus.Cancelled)
        //        return (null, "Cannot pay for a cancelled order.");

        //    var payment = new Payment
        //    {
        //        OrderId = order.Id,
        //        Method = dto.Method,
        //        Amount = order.Total,
        //        Status = PaymentStatus.Completed,
        //        PaidAt = DateTime.UtcNow
        //    };

        //    order.Status = OrderStatus.Processing;

        //    _context.Payments.Add(payment);
        //    await _context.SaveChangesAsync();

        //    return (ToDto(payment), null);
        ////}

        public async Task<(PaymentIntentDto? Result, string? Error)> InitiateAsync(string userId, CreatePaymentDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order is null) return (null, "Order not found.");
            if (order.UserId != userId) return (null, "Forbidden.");
            if (order.Payment is not null) return (null, "Order already has a payment.");
            if (order.Status == OrderStatus.Cancelled)
                return (null, "Cannot pay for a cancelled order.");

            // Use orderId as the unique reference Paymob stores
            var reference = $"ORDER-{order.Id}-{DateTime.UtcNow.Ticks}";
            var clientSecret = await _paymobService.CreatePaymentIntentionAsync(order.Total, reference);

            // Save a pending payment record
            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = order.Total,
                Status = PaymentStatus.Pending,
                Reference = reference     // ← add this field to Payment model
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return (new PaymentIntentDto(clientSecret, _paymob.PublicKey, order.Id), null);
        }

        private static PaymentDto ToDto(Payment p) => new( p.Id, p.OrderId, p.Method, p.Status, p.Amount, p.PaidAt );
    }
}
