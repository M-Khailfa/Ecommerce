using Ecommerce.Core.Dtos.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public decimal Total { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = OrderStatus.Pending.ToString();

        [MaxLength(500)]
        public string? ShippingAddress { get; set; }

        public DateTime PlacedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public AppUser User { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = [];
        public Payment? Payment { get; set; }
    }
}
