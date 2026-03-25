using MusicBackend.Models.Data.User;
using MusicBackend.Models.DataLayer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using MusicBackend.Models.Data;

namespace MusicBackend.Services.TokenGenerator
{
    public class JWTTokenGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        public JWTTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        
        async public Task<string> GenerateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsSuper.ToString()),
                new Claim(ClaimTypes.Sid, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtConfig:Key").Value));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);
            
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        async public Task<RefreshToken> GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow
            };
            return refreshToken;
        }

        async public Task SetRefreshToken(RefreshToken newRefreshToken, HttpResponse Response)
        {
            var cookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = newRefreshToken.Expires,
                Path = "/"
            };
            cookie.Extensions.Add("Partitioned");

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookie);
        }

        async public Task<ClaimData> GetClaims(ClaimsPrincipal user)
        {
            string username = user.FindFirst(ClaimTypes.Name)?.Value;
            string isSuperAsString = user.FindFirst(ClaimTypes.Role)?.Value;
            string userId = user.FindFirst(ClaimTypes.Sid)?.Value;
            if (isSuperAsString == null || username == null || userId == null)
            {
                return null;
            }
            bool isSuper;
            bool.TryParse(isSuperAsString, out isSuper);
            ClaimData claimData = new()
            {
                Name = username,
                Id = userId,
                IsSuper = isSuper
            };
            return claimData;
        }
    }
}
