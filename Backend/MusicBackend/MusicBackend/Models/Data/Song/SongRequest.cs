namespace MusicBackend.Models.Data.Song
{
    public class SongRequest
    {
        public string? Title { get; set; }
        public string? Composer { get; set; }
        public IFormFile? Thumbnail { get; set; }

        public bool GetIsFilled()
        {
            return (Title != null && Composer != null && Thumbnail != null);
        }
    }
}
