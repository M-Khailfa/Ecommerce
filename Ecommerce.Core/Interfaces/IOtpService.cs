using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Interfaces
{
    public interface IOtpService
    {
        string GenerateAndStoreOtp(string email);
        bool ValidateOtp(string email, string providedOtp);
    }
}
