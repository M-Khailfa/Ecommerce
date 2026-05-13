using Ecommerce.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Infrastructure.Repos
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _expirationTime = TimeSpan.FromMinutes(10);

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string GenerateAndStoreOtp(string email)
        {
            var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString();
            _cache.Set($"OTP_{email.ToLowerInvariant()}", otpCode, _expirationTime);
            return otpCode;
        }

        public bool ValidateOtp(string email, string providedOtp)
        {
            var cacheKey = $"OTP_{email.ToLowerInvariant()}";
            if (_cache.TryGetValue(cacheKey, out string? storedOtp) && storedOtp == providedOtp)
            {
                _cache.Remove(cacheKey);
                return true;
            }
            return false;
        }
    }
}
