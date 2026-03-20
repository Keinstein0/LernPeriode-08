using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicBackend.Models;
using MusicBackend.Models.Data;
using MusicBackend.Models.Data.Playlist;
using MusicBackend.Models.DataLayer;
using MusicBackend.Services;
using System.Threading.Tasks;

namespace MusicBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistController : ControllerBase
    {
        private ITokenGenerator _tokenGenerator;
        private MusicContext _context;
        public PlaylistController(ITokenGenerator tokenGenerator, MusicContext context)
        {
            _tokenGenerator = tokenGenerator;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostPlaylist(PlaylistRequest request)
        {
            ClaimData claims = await _tokenGenerator.GetClaims(User);

            User? user = await _context.Users.FirstOrDefaultAsync(user => user.Id == claims.Id);
            if (user == null)
            {
                return NotFound("What de heel, if you see this then i'll be utterly and completely dumbfounded");
            }

            Playlist playlist = new Playlist()
            {
                Name = request.Name,
                Id = Guid.NewGuid().ToString(),
                Owner = user,
                CreatedDate = DateTime.UtcNow,
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return Ok(playlist);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlaylist(PlaylistRequest request, [FromRoute] string id)
        {
            ClaimData claims = await _tokenGenerator.GetClaims(User);
            
            Playlist? playlist = await _context.Playlists.FirstOrDefaultAsync(playlist => playlist.Id == id && playlist.OwnerId == claims.Id);
            if (playlist == null)
            {
                return NotFound("Playlist not found");
            }


            playlist.Name = request.Name;
            await _context.SaveChangesAsync();
            return Ok(playlist);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist([FromRoute] string id)
        {
            ClaimData claim = await _tokenGenerator.GetClaims(User);
            
            Playlist? playlist = await _context.Playlists.FirstOrDefaultAsync(playlist => playlist.Id == id && playlist.OwnerId == claim.Id);

            if (playlist == null) { return NotFound(); }

            _context.Remove(playlist);

            return Ok(playlist);
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylists([FromQuery] string filter)
        {
            IQueryable<Playlist> query = _context.Playlists;
            ClaimData claims = await _tokenGenerator.GetClaims(User);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(p => p.Name.Contains(filter) && p.OwnerId == claims.Id);
            }

            query = query.OrderBy(p => p.CreatedDate);

            List<Playlist> playlists = await query.ToListAsync();

            List<DisplayPlaylist> displayPlaylists = Playlist.AsDisplayPlaylistList(playlists);

            return Ok(displayPlaylists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongsInPlaylist([FromRoute] string id)
        {
            ClaimData claims = await _tokenGenerator.GetClaims(User);
            
            Playlist? playlist = await _context.Playlists.FirstOrDefaultAsync(playlist => playlist.Id == id && playlist.OwnerId == claims.Id);

            if (playlist == null) { return NotFound(); }

            SongPlaylist displayPlaylist = new SongPlaylist(playlist);

            return Ok(displayPlaylist);
        }

        [HttpPost("{list_id}/{music_id}")]
        public async Task<IActionResult> PostSongInPlaylist([FromRoute] string list_id, [FromRoute] string music_id)
        {
            ClaimData claims = await _tokenGenerator.GetClaims(User);

            // 3 queries for every bit of data
            Song? song = await _context.Songs.FirstOrDefaultAsync(song => song.Id == music_id);
            Playlist? playlist = await _context.Playlists.FirstOrDefaultAsync(playlist => playlist.Id == list_id && 
                                                                              playlist.OwnerId == claims.Id);

            if (song == null) { return NotFound(); }
            if (playlist == null) { return NotFound(); }

            List<PlaylistSong> playlistSongList = await _context.PlaylistSongs.Where(song => song.PlaylistId == playlist.Id)
                                                                              .OrderByDescending(song => song.Index).ToListAsync();

            PlaylistSong playlistSong = new PlaylistSong()
            {
                Playlist = playlist,
                PlaylistId = list_id,

                Song = song,
                SongId = music_id,
                
                Index = playlistSongList[0].Index + 1 //Get the largest index and add one (adds the item to the bottom)
            };

            _context.PlaylistSongs.Add(playlistSong);

            return Ok(song);
        }

        [HttpPut("{list_id}/{music_id}")]
        public async Task<IActionResult> UpdateSongInPlaylist([FromRoute] string list_id, [FromRoute] string music_id, PlaylistSongRequest request)
        {
            return Ok("bruhhhhhh");
        }



    }
}
