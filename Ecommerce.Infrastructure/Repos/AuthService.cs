using Ecommerce.Core.Dtos.Auth;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Infrastructure.Repos
{
    public class AuthService : IAuthService
    {
        public readonly UserManager<AppUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly JWT _jwt;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        public AuthService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, IEmailService emailService, IOtpService otpService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _jwt = jwt.Value;
            _emailService = emailService;
            _otpService = otpService;
        }
        public async Task<AuthDto> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userManager.FindByEmailAsync(registerDto.Email) is not null)
                return new AuthDto { Message = "Email is already registered!" };
            if (await _userManager.FindByNameAsync(registerDto.Username) is not null)
                return new AuthDto { Message = "Username is already registered!" };

            var user = new AppUser
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = "";
                foreach (var error in result.Errors)
                    errors += $"{error.Description} ";
                return new AuthDto { Message = $"User creation failed! Errors: {errors}" };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var jwtSecurityToken = await CreateJwtToken(user);

            return new AuthDto
            {
                Email = user.Email,
                Username = user.UserName,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Roles = new List<string> { "User" }
            };

        }

        public async Task<AuthDto> LoginAsync(LoginDto loginDto)
        {
            var authModel = new AuthDto();
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            var otp = _otpService.GenerateAndStoreOtp(user.Email);
            string emailBody = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2>Your Security Code</h2>
                <p>Please use the following 6-digit code to complete your login:</p>
                <h1 style='color: #007bff; letter-spacing: 5px;'>{otp}</h1>
                <p>This code expires in 10 minutes.</p>
            </div>";

            await _emailService.SendEmailAsync(user.Email, "Ecommerce Login Verification", emailBody);
            authModel.RequiresOTP = true;
            authModel.Email = user.Email;
            authModel.Message = "Credentials verified. Waiting for OTP Verification.";

            return authModel;
        }

        public async Task<AuthDto> VerifyOtpAsync(VerifyOtpDto verifyDto)
        {
            var authModel = new AuthDto();

            var isValid = _otpService.ValidateOtp(verifyDto.Email, verifyDto.OtpCode);
            if (!isValid)
            {
                authModel.Message = "Invalid or expired OTP.";
                return authModel;
            }

            var user = await _userManager.FindByEmailAsync(verifyDto.Email);
            if (user == null)
            {
                authModel.Message = "User not found.";
                return authModel;
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Roles = roles.ToList();
            authModel.RequiresOTP = false;

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }

        public async Task<AuthDto> RefreshTokenAsync(string token)
        {
            var authModel = new AuthDto();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(rf => rf.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid refresh token!";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive refresh token!";
                return authModel;
            }

            // Revoke current refresh token
            refreshToken.RevokedOn = DateTime.UtcNow;

            // Generate new Refresh Token and Append it to User
            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            // Generate new JWT Token
            var jwtToken = await CreateJwtToken(user);

            //Initialize AuthModel and return it
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(rf => rf.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(rt => rt.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<AuthDto> ForgotPasswordAsync(string email)
        {
            var authModel = new AuthDto();
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                authModel.Message = "If an account matches that email, a password reset code has been sent.";
                return authModel;
            }

            var otp = _otpService.GenerateAndStoreOtp(user.Email);

            string emailBody = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 10px; max-width: 600px;'>
                    <h2 style='color: #333;'>Password Reset Request</h2>
                    <p>We received a request to reset your password. Please use the following 6-digit code:</p>
                    <div style='background-color: #f4f4f4; padding: 15px; text-align: center; border-radius: 5px; margin: 20px 0;'>
                        <span style='font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #dc3545;'>{otp}</span>
                    </div>
                    <p style='font-size: 12px; color: #777;'>This code expires in 10 minutes. If you did not request a password reset, please ignore this email.</p>
                </div>";

            await _emailService.SendEmailAsync(user.Email, "Reset Your Password", emailBody);

            authModel.Message = "If an account matches that email, a password reset code has been sent.";
            return authModel;
        }

        public async Task<AuthDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var authModel = new AuthDto();

            var isValid = _otpService.ValidateOtp(resetPasswordDto.Email, resetPasswordDto.OtpCode);
            if (!isValid)
            {
                authModel.Message = "Invalid or expired OTP.";
                return authModel;
            }

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                authModel.Message = "User not found.";
                return authModel;
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                authModel.Message = $"Failed to reset password: {errors}";
                return authModel;
            }

            authModel.IsAuthenticated = true;
            authModel.Message = "Password has been reset successfully. You can now log in.";

            return authModel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(AppUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        private RefreshTokenDto GenerateRefreshToken()
        {
            var randomNumber = RandomNumberGenerator.GetBytes(32);
            return new RefreshTokenDto
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
