using Ecommerce.Core.Dtos.Auth;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Ecommerce.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = new ApiResponse();

            var result = await _authService.RegisterAsync(registerDto);
            if (!result.IsAuthenticated)
            {
                response = ApiResponse.BadRequest(result.Message);
                return BadRequest(response);
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            response = ApiResponse.Created(result.Email, result.Message);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = new ApiResponse();

            var result = await _authService.LoginAsync(loginDto);

            if (result.RequiresOTP)
            {
                response = ApiResponse.Success(new { result.Email, result.RequiresOTP }, result.Message);
                return Ok(response);
            }

            if (!result.IsAuthenticated && !result.RequiresOTP)
                return Unauthorized(result.Message);

            response = ApiResponse.BadRequest("No Need For OTP!");
            return BadRequest(response);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto verifyOtpDto)
        {
            var response = new ApiResponse();
            var result = await _authService.VerifyOtpAsync(verifyOtpDto);

            if (!result.IsAuthenticated)
                return Unauthorized(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            response = ApiResponse.Success(result, "OTP verified successfully. Logged in.");
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var response = new ApiResponse();
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                response = ApiResponse.BadRequest("No refresh token provided.");
                return BadRequest(response);
            }

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
            {
                response = ApiResponse.BadRequest(result.Message);
                return BadRequest(response);
            }

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            }

            response = ApiResponse.Success(result, "Token refreshed successfully");
            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDto revokeTokenDto)
        {
            var response = new ApiResponse();
            var token = revokeTokenDto.token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
            {
                response = ApiResponse.BadRequest("No token provided.");
                return BadRequest(response);
            }
            var result = await _authService.RevokeTokenAsync(token);
            if (!result)
            {
                response = ApiResponse.BadRequest("Token revocation failed.");
                return BadRequest(response);
            }
            Response.Cookies.Delete("refreshToken");
            response = ApiResponse.Success("Token revoked successfully.");
            return Ok(response);
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}
