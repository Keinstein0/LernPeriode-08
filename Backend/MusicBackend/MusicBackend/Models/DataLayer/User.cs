using MusicBackend.Models.Data.User;

namespace MusicBackend.Models.DataLayer
{
    public class User
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public required string Hash { get; set; }
        public bool IsSuper { get; set; }

        // JWT Thingies //
        public string? RefreshToken { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime TokenExpires { get; set; }

        // Relations //
        public ICollection<Playlist> Playlists { get; set; } = new List<Playlist>();

        public DisplayUser AsDisplayUser()
        {
            DisplayUser display = new DisplayUser() { Id = Id , Username = Username, IsSuper = IsSuper};
            return display;
        }
    }
}
