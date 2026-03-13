namespace MusicBackend.Services
{
    public interface IFetchInterface
    {
        public Task<IFormFile> GetResource(string url);
        public Task<bool> ResourceExists(string url);
    }
}
