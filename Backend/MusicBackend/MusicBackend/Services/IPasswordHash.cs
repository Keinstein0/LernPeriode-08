namespace HPlusSportAPI.Services
{
    public interface IPasswordHash
    {
        public Task<string> HashPassword(string password);
        public Task<bool> VerifyPasssword(string password, string passwordHash);
    }

}
