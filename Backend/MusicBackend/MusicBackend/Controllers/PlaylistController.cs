using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MusicBackend.Models;
using MusicBackend.Models.Data;
using MusicBackend.Models.Data.Playlist;
using MusicBackend.Models.DataLayer;
using MusicBackend.Services.TokenGenerator;
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

        [Authorize]
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
                OwnerId = user.Id,
                CreatedDate = DateTime.UtcNow,
            };

            _context.Playlists.Add(playlist);
            await _context.SaveChangesAsync();

            return Ok(playlist.AsDisplayPlaylist());
        }

        [Authorize]
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
            return Ok(playlist.AsDisplayPlaylist());
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlaylist([FromRoute] string id)
        {
            ClaimData claim = await _tokenGenerator.GetClaims(User);
            
            Playlist? playlist = await _context.Playlists.FirstOrDefaultAsync(playlist => playlist.Id == id && playlist.OwnerId == claim.Id);

            if (playlist == null) { return NotFound(); }

            _context.Remove(playlist);
            await _context.SaveChangesAsync();

            return Ok(playlist.AsDisplayPlaylist());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetPlaylists([FromQuery] string? filter)
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

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSongsInPlaylist([FromRoute] string id)
        {
            ClaimData claims = await _tokenGenerator.GetClaims(User);
            
            Playlist? playlist = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .ThenInclude(ps => ps.Song)
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == claims.Id);



            if (playlist == null) { return NotFound(); }

            DisplaySongPlaylist displayPlaylist = new DisplaySongPlaylist(playlist);

            return Ok(displayPlaylist);
        }

        [Authorize]
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

            int newIndex = 0;
            if (playlistSongList.Count() > 0) 
            { 
                newIndex = playlistSongList[0].Index + 1; 
            }

            PlaylistSong playlistSong = new PlaylistSong()
            {
                Playlist = playlist,
                PlaylistId = list_id,

                Song = song,
                SongId = music_id,
                
                Index = newIndex //Get the largest index and add one (adds the item to the bottom)
            };

            if (playlistSongList.Any(o => o.SongId == music_id))
            {
                return BadRequest("Song already in playlist");
            }

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            return Ok(song.AsDisplaySong());
        }

        [Authorize]
        [HttpPut("{list_id}/{music_id}")]
        public async Task<IActionResult> UpdateSongInPlaylist([FromRoute] string list_id, [FromRoute] string music_id, PlaylistSongRequest request)
        {
            ClaimData claims = await _tokenGenerator.GetClaims(User);
            int targetIndex = (int)Math.Floor(request.Index);

            Playlist? play = await _context.Playlists.FirstOrDefaultAsync(playlist => playlist.Id == list_id && playlist.OwnerId == claims.Id);
            if (play == null) { return NotFound(); } // Trying to acess playlist of someone else and we can't have that, can we?

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var songItem = await _context.PlaylistSongs.FirstOrDefaultAsync(playlistSong => playlistSong.PlaylistId == list_id && playlistSong.SongId == music_id);
                if (songItem == null) { return NotFound(); }

                int oldIndex = songItem.Index;

                if (targetIndex < oldIndex) // Move up
                {
                    await _context.PlaylistSongs
                        .Where(x => x.Index >= targetIndex && x.Index < oldIndex)
                        .ExecuteUpdateAsync(s => s.SetProperty(b => b.Index, b => b.Index + 1));
                }
                else if (targetIndex > oldIndex) // Move down
                {
                    await _context.PlaylistSongs
                        .Where(x => x.Index > oldIndex && x.Index <= targetIndex)
                        .ExecuteUpdateAsync(s => s.SetProperty(b => b.Index, b => b.Index - 1));
                }

                songItem.Index = targetIndex;
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(ex.Message);
            }
            
            
            return Ok();
        }

        [Authorize]
        [HttpDelete("{list_id}/{music_id}")]
        public async Task<IActionResult> DeleteSongFromPlaylist([FromRoute] string list_id, [FromRoute] string music_id)
        {
            ClaimData claim = await _tokenGenerator.GetClaims(User);

            Playlist? playlist = await _context.Playlists.FirstOrDefaultAsync(playlist => playlist.Id == list_id && playlist.OwnerId == claim.Id);
            if (playlist == null) { return NotFound(); }

            PlaylistSong? playlistSong = _context.PlaylistSongs.FirstOrDefault(song => song.PlaylistId == list_id && song.SongId == music_id);

            if (playlistSong == null) { return NotFound(); }

            _context.Remove(playlistSong);
            await _context.SaveChangesAsync();

            return Ok(playlist.AsDisplayPlaylist());
        }
    }
}
