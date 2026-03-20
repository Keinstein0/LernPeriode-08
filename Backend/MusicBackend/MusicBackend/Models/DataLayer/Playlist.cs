using MusicBackend.Models.Data.Playlist;

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
        public ICollection<Song> Songs { get; } = new List<Song>();
        public ICollection<PlaylistSong> PlaylistSongs { get; } = new List<PlaylistSong>();

        public DisplayPlaylist AsDisplayPlaylist()
        {
            return new DisplayPlaylist()
            {
                Id = Id,
                Name = Name,
                Created = CreatedDate
            };
        }

        public static List<DisplayPlaylist> AsDisplayPlaylistList(List<Playlist> list)
        {
            List<DisplayPlaylist> displayPlaylistList = new List<DisplayPlaylist>();
            foreach (Playlist playlist in list)
            {
                displayPlaylistList.Add(playlist.AsDisplayPlaylist());
            }
            return displayPlaylistList;
        }
    }
}
