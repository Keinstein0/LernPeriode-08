namespace MusicBackend.Models.Data.Song
{
    public class FetchRequest
    {
        public required string MusicUrl { get; set; }
        public required string Title { get; set; }
        public IFormFile? ThumbnailStream { get; set; }
        public string? Composer { get; set; }
    }
}
