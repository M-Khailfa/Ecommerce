using Ecommerce.Core.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ecommerce.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthDto> LoginAsync(LoginDto loginDto);
        Task<AuthDto> VerifyOtpAsync(VerifyOtpDto verifyDto);
        Task<AuthDto> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
