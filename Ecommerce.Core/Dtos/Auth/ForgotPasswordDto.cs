using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Dtos.Auth
{
    public class ForgotPasswordDto
    {
        public required string Email { get; set; }
    }
}
