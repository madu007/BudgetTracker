using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Model.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BudgetTracker.Domain.Services
{
    public interface ITokenService
    {
        //string CreateJWTToken(IdentityUser user, List<string> roles);
        Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user);
        Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens);

    }
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        //public string CreateJWTToken(IdentityUser user, List<string> roles)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Authentication:JwtBearer:SecretKey").Value);
        //    var minutres = Convert.ToDouble(_configuration.GetSection("Authentication:JwtBearer:AccessExpiration").Value);
        //    var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Email, user.Email)
        //        };

        //    foreach (var role in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(claims),

        //        Expires = DateTime.UtcNow.AddMinutes(minutres),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
        //        SecurityAlgorithms.HmacSha256Signature),
        //        Audience = _configuration.GetSection("Authentication:JwtBearer:Audience").Value,
        //        Issuer = _configuration.GetSection("Authentication:JwtBearer:Issuer").Value
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}

        public async Task<ApiResponse<LoginResponse>> GetJwtTokenAsync(ApplicationUser user)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = GetToken(authClaims); //access token
            var refreshToken = GenerateRefreshToken();
            _ = int.TryParse(_configuration["Authentication:JwtBearer:RefreshTokenValidity"], out int refreshTokenValidity);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenValidity);

            await _userManager.UpdateAsync(user);

            return new ApiResponse<LoginResponse>
            {
                Response = new LoginResponse()
                {
                    AccessToken = new TokenType()
                    {
                        Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        ExpiryTokenDate = jwtToken.ValidTo
                    },
                    RefreshToken = new TokenType()
                    {
                        Token = user.RefreshToken,
                        ExpiryTokenDate = (DateTime)user.RefreshTokenExpiry
                    }
                },

                IsSuccess = true,
                StatusCode = 200,
                Message = $"Token created"
            };
        }



        public async Task<ApiResponse<LoginResponse>> RenewAccessTokenAsync(LoginResponse tokens)
        {
            var accessToken = tokens.AccessToken;
            var refreshToken = tokens.RefreshToken;
            var principal = GetClaimsPrincipal(accessToken.Token);
            var user = await _userManager.FindByEmailAsync(principal.Identity.Name);
            if (refreshToken.Token != user.RefreshToken && refreshToken.ExpiryTokenDate <= DateTime.Now)
            {
                return new ApiResponse<LoginResponse>
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "Token is invalid or has expired"
                };
            }
            var result = await GetJwtTokenAsync(user);
            return result;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JwtBearer:SecretKey"]));
            _ = int.TryParse(_configuration["Authentication:JwtBearer:TokenValidityInMinutes"], out int tokenValidityInMinutes);
            var expirationTime = Convert.ToDouble(_configuration["Authentication:JwtBearer:TokenValidityInMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:JwtBearer:Issuer"],
                audience: _configuration["Authentication:JwtBearer:Audience"],
                expires: DateTime.UtcNow.AddMinutes(expirationTime),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal GetClaimsPrincipal(string accessToken)
        {
            var tokenValidateParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:JwtBearer:SecretKey"])),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principle = tokenHandler.ValidateToken(accessToken, tokenValidateParameters, out SecurityToken securityToken);
            return principle;
        }
    }
}
