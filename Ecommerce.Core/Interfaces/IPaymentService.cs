using Ecommerce.Core.Dtos.Payment;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto?> GetByOrderAsync(int orderId);
        Task<(PaymentIntentDto? Result, string? Error)> InitiateAsync(string userId, CreatePaymentDto dto);
    }
}
