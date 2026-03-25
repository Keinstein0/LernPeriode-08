using MusicBackend.Models.DataLayer;
using System.Diagnostics.CodeAnalysis;

namespace MusicBackend.Models.Data.Song
{
    public class PlaylistSongItem
    {
        public required string Id { get; set; }
        public string? ThumbnailUrl { get; set; }
        public required string Title { get; set; }
        public string? Composer { get; set; }
        public int Length { get; set; }
        public int Index { get; set; }

        [SetsRequiredMembers]
        public PlaylistSongItem(PlaylistSong connector)
        {
            DataLayer.Song song = connector.Song;

            Id = song.Id;
            ThumbnailUrl = song.ThumbnailUrl;
            Title = song.Title;
            Composer = song.Composer;
            Length = song.Length;

            Index = connector.Index;
        }

        public static List<PlaylistSongItem> ConvertToPlaylistSongItem(List<PlaylistSong> songConnectors)
        {
            List<PlaylistSongItem> songItems = new List<PlaylistSongItem>();
            foreach (var connector in songConnectors)
            {
                songItems.Add(new PlaylistSongItem(connector));
            }
            songItems.OrderBy(o => o.Index).ToList();

            return songItems;
        }
    }
}
