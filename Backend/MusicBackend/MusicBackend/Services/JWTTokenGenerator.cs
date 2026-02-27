using HPlusSportAPI.Services;

namespace MusicBackend.Services
{
    public class JWTTokenGenerator : ITokenGenerator
    {
        async public string GenerateAccessToken(User user)
        {
            throw new NotImplementedException();
        }

        async public RefreshToken GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
