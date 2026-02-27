namespace MusicBackend.Models.DataLayer
{
    public class Song
    {
        public required string Id { get; set; }
        public required string MusicUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public required string Title { get; set; }
        public string? Composer { get; set; }
        public int Length { get; set; }

        public required ICollection<Playlist> playlists { get; set; }
    }
}
