namespace MusicBackend.Models.Data.Song
{
    public class SongAuthorisation
    {
        public required string MusicUrl { get; set; }
        public required string Token { get; set; }
        public int Length { get; set; }
    }
}
