namespace MusicBackend.Services.Fetcher
{
    public class MockFetchInterface : IFetchInterface
    {
        public async Task<IFormFile> GetResource(string url)
        {
            return null; //nahh "possibly a null ref return" what else could it be C#
        }

        public async Task<bool> ResourceExists(string url)
        {
            return true;
        }
    }
}
