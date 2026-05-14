using Ecommerce.Core.Dtos.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ecommerce.Core.Dtos.Payment
{
    public record CreatePaymentDto(
        [Required] int OrderId,
        [Required] PaymentMethod Method
    );

    public record PaymentDto(
        int Id,
        int OrderId,
        PaymentMethod Method,
        PaymentStatus Status,
        decimal Amount,
        DateTime? PaidAt
    );
}
