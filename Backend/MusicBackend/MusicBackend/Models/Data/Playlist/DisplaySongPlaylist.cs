using MusicBackend.Models.Data.Song;
using MusicBackend.Models.DataLayer;

namespace MusicBackend.Models.Data.Playlist
{
    public class DisplaySongPlaylist
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Created { get; set; }

        public List<PlaylistSongItem>? Songs { get; set; }

        public DisplaySongPlaylist(DataLayer.Playlist playlist)
        {
            Songs = PlaylistSongItem.ConvertToPlaylistSongItem(playlist.PlaylistSongs.ToList());

            Id = playlist.Id;
            Name = playlist.Name;
            Created = playlist.CreatedDate;
        }
    }
}
