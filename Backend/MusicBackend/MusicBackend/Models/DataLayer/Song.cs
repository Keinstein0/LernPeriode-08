using MusicBackend.Models.Data.Song;

namespace MusicBackend.Models.DataLayer
{
    public class Song
    {
        public required string Id { get; set; }
        public required string MusicUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public required string Title { get; set; }
        public required string Composer { get; set; }
        public int Length { get; set; }

        public ICollection<Playlist> Playlists { get; } = new List<Playlist>();
        public ICollection<PlaylistSong> PlaylistSongs { get; } = new List<PlaylistSong>();

        public DisplaySong AsDisplaySong()
        {
            return new DisplaySong
            { Id = Id,
            MusicUrl = MusicUrl,
            ThumbnailUrl = ThumbnailUrl,
            Title = Title,
            Composer = Composer,
            Length = Length,
            };
        }
    }
}