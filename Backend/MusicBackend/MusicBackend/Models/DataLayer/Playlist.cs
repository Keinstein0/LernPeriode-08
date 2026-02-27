namespace MusicBackend.Models.DataLayer
{
    public class Playlist
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        // Ownership
        public required User Owner { get; set; }
        public string? OwnerId { get; set; }
        public ICollection<Song>? Songs { get; set; } = null;
    }
}
