using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Interfaces
{
    public interface IPaymobService
    {
        Task<string> CreatePaymentIntentionAsync(decimal totalAmount, string orderReference);
    }
}
