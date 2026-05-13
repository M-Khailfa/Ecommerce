using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Dtos.Auth
{
    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
    }
}
