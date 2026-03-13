namespace MusicBackend.Models.DataLayer
{
    public class PlaylistSong
    {
        public required string SongId { get; set; }
        public Song Song { get; set; } = null!;

        public required string PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;

        public int Index { get; set; }
    }
}
