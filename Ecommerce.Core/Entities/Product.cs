using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Ecommerce.Core.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Brand { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public Category Category { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = [];
        public ICollection<Review> Reviews { get; set; } = [];
    }
}
