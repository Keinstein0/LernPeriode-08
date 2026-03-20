namespace MusicBackend.Models.Data.Playlist
{
    public class DisplayPlaylist
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public DateTime? Created { get; set; } 
    }
}
