namespace MusicBackend.Models
{
    public class User
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public required string Hash { get; set; }
        public bool IsSuper { get; set; }

        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
