using HPlusSportAPI.Services;
using BCrypt.Net;

namespace MusicBackend.Services
{
    public class BCryptHasher : IPasswordHash
    {
        async public Task<string> HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        async public Task<bool> VerifyPasssword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
