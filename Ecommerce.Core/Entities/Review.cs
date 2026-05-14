using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ecommerce.Core.Entities
{
    public class Review
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Product Product { get; set; } = null!;
        public AppUser User { get; set; } = null!;
    }
}
