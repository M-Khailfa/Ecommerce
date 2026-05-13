using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Dtos.Auth
{
    public class ResetPasswordDto
    {
        public required string Email { get; set; }
        public required string OtpCode { get; set; }
        public required string NewPassword { get; set; }
    }
}
