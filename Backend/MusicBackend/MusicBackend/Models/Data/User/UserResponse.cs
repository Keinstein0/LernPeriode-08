namespace MusicBackend.Models.Data.User
{
    public class UserResponse
    {
        public bool IsSuper {  get; set; }
        public required string Uid { get; set; }
        public required string Token {  get; set; }
    }
}
