using Ecommerce.Core.Dtos.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.Core.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        [Required, MaxLength(50)]
        public PaymentMethod Method { get; set; } = PaymentMethod.COD;

        [Required, MaxLength(50)]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public decimal Amount { get; set; }

        public DateTime? PaidAt { get; set; }
        public string? Reference { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
