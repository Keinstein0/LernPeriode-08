using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicBackend.Models;
using MusicBackend.Models.Data.Song;
using MusicBackend.Models.Data.User;
using MusicBackend.Models.DataLayer;
using MusicBackend.Services;

namespace MusicBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private MusicContext _context;
        private IBucketInterface _bucketInterface;
        private IFetchInterface _fetchInterface;
        
        public SongController(MusicContext context, IBucketInterface bucketInterface, IFetchInterface fetchInterface)
        {
            _context = context;
            _bucketInterface = bucketInterface;
            _fetchInterface = fetchInterface;
        }

        [Authorize]
        [HttpGet]
        async public Task<IActionResult> GetSongs([FromQuery] int page, [FromQuery] int length, [FromQuery] string filter)
        {
            IQueryable<Song> query = _context.Songs;

            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(p => p.Title.Contains(filter) || p.Composer.Contains(filter));
            }

            query = query.OrderBy(p => p.Title);

            List<Song> songs = await query
                .Skip((page - 1) * length)
                .Take(length)
                .ToListAsync();

            List<DisplaySong> displaySongs;
            DisplaySong.ToDisplayUsers(songs, out displaySongs);
            return Ok(displaySongs);
        }

        [HttpGet("{id}")]
        async public Task<IActionResult> GetSong([FromRoute] string id)
        {
            Song? song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            DisplaySong displaySong = song.AsDisplaySong();
            return Ok(displaySong);
        }

        [Authorize]
        [HttpGet("{id}/authorize")]
        async public Task<IActionResult> GetSongAuthorisation([FromRoute] string id)
        {
            Song? song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            string token = await _bucketInterface.GetAuthorisationForFile(song.MusicUrl);
            SongAuthorisation authorisation = new SongAuthorisation()
            {
                MusicUrl = song.MusicUrl,
                Token = token,
                Length = song.Length
            };
            return Ok(authorisation);
        }

        [HttpPut("{id}")]
        async public Task<IActionResult> UpdateSong([FromRoute] string id,[FromForm] SongRequest songRequest)
        {
            Song? song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            if (songRequest.Title != null)
            {
                song.Title = songRequest.Title;
            }

            if (songRequest.Composer != null)
            {
                song.Composer = songRequest.Composer;
            }

            if (songRequest.Thumbnail != null)
            {
                string url = await _bucketInterface.UploadThumbnail(songRequest.Thumbnail, id);
                song.ThumbnailUrl = url;
            }
            await _context.SaveChangesAsync();

            return Ok(song.Id);
        }

        [HttpDelete("{id}")]
        async public Task<IActionResult> DeleteSong([FromRoute] string id)
        {
            Song? song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            await _bucketInterface.DeleteResource(song.MusicUrl);
            await _bucketInterface.DeleteResource(song.ThumbnailUrl);

            _context.Remove(song);
            await _context.SaveChangesAsync();

            return Ok(song);
        }

        [HttpPost]
        public async Task<IActionResult> FetchSongAndUpload(FetchRequest request)
        {
            if (!await _fetchInterface.ResourceExists(request.MusicUrl))
            {
                return NotFound("URL not found/not viable for import of file");
            }
            string id = Guid.NewGuid().ToString();
            if (request.Composer == null)
            {
                request.Composer = "Unknown";
            }
            string? thumbnailUrl = null;
            if (request.ThumbnailStream != null)
            {
                thumbnailUrl = await _bucketInterface.UploadThumbnail(request.ThumbnailStream, id);
            }

            var stream = await _fetchInterface.GetResource(request.MusicUrl);
            string url = await _bucketInterface.UploadMusic(stream, id);


            Song song = new Song()
            {
                Id = id,
                MusicUrl = url,
                ThumbnailUrl = thumbnailUrl,
                Title = request.Title,
                Composer = request.Composer,
                Length = 1
            };
            _context.Songs.Add(song);
            await _context.SaveChangesAsync(true);

            return Ok(song);
        }
    }
}
