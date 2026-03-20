using MusicBackend.Models.Data.Song;
using MusicBackend.Models.DataLayer;

namespace MusicBackend.Models.Data.Playlist
{
    public class SongPlaylist
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Created { get; set; }

        public List<DisplaySong>? Songs { get; set; }

        public SongPlaylist(DataLayer.Playlist playlist)
        {
            Songs = DisplaySong.ToDisplayUsers(playlist.Songs.ToList()); //lwky if this works

            Id = playlist.Id;
            Name = playlist.Name;
            Created = playlist.CreatedDate;
        }
    }
}
