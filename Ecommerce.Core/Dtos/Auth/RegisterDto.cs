using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Ecommerce.Core.Dtos.Auth
{
    public class RegisterDto
    {
        [Required, StringLength(50)]
        public string FullName { get; set; }
        [Required, StringLength(50)]
        public string Username { get; set; }
        [Required, StringLength(50)]
        public string Email { get; set; }
        [Required, StringLength(50)]
        public string Password { get; set; }
    }
}
