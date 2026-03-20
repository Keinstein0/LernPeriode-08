using MusicBackend.Models.Data.User;
using MusicBackend.Models.DataLayer;

namespace MusicBackend.Models.Data.Song
{
    public class DisplaySong
    {
        public required string Id { get; set; }
        public required string MusicUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public required string Title { get; set; }
        public string? Composer { get; set; }
        public int Length { get; set; }


        public static List<DisplaySong> ToDisplayUsers(List<MusicBackend.Models.DataLayer.Song> users)
        {
            List<DisplaySong> displayUsers = new List<DisplaySong>();

            foreach (var user in users)
            {
                displayUsers.Add(user.AsDisplaySong());
            }

            return displayUsers;
        }
    }
}
