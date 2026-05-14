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
        public string Method { get; set; } = PaymentMethod.COD.ToString();

        [Required, MaxLength(50)]
        public string Status { get; set; } = PaymentStatus.Pending.ToString();

        public decimal Amount { get; set; }

        public DateTime? PaidAt { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
    }
}
