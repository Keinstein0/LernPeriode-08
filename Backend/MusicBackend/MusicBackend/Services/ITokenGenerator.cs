using JWTwebapp.Datamodels;

namespace HPlusSportAPI.Services
{
    public interface ITokenGenerator
    {
        public string GenerateAccessToken(User user);
        public RefreshToken GenerateRefreshToken();
    }

}
