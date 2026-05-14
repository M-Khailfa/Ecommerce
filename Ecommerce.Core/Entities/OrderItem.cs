using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        [NotMapped]
        public decimal LineTotal => UnitPrice * Quantity;

        // Navigation
        public Order Order { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}
