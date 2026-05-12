
namespace MusicBackend.Services.Fetcher
{
    public class YouTubeFetcher : IFetchInterface
    {
        public Task<IFormFile> GetResource(string url)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResourceExists(string url)
        {
            throw new NotImplementedException();
        }
    }
}
