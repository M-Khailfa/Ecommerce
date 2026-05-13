using Ecommerce.Core.Dtos.Auth;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ecommerce.Core.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(30)]
        public string FullName { get; set; }
        public List<RefreshTokenDto>? RefreshTokens { get; set; }
    }
}
