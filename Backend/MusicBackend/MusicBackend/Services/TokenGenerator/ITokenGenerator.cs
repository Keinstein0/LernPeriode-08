using MusicBackend.Models.Data;
using MusicBackend.Models.Data.User;
using MusicBackend.Models.DataLayer;

namespace MusicBackend.Services.TokenGenerator
{
    public interface ITokenGenerator
    {
        public Task<string> GenerateAccessToken(User user);
        public Task<RefreshToken> GenerateRefreshToken();
        public Task SetRefreshToken(RefreshToken newRefreshToken, HttpResponse Response);
        public Task<ClaimData> GetClaims(System.Security.Claims.ClaimsPrincipal user);
    }
}
