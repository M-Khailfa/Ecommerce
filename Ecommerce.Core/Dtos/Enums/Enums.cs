using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Dtos.Enums
{
    public enum OrderStatus { Pending, Processing, Shipped, Delivered, Cancelled }
    public enum PaymentStatus { Pending, Completed, Failed, Refunded }
    public enum PaymentMethod { CreditCard, PayPal, Stripe, COD }
}
