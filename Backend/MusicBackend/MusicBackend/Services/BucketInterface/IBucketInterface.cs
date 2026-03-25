namespace MusicBackend.Services.BucketInterface
{
    public interface IBucketInterface
    {
        // Thumbnail
        public Task<string> UploadThumbnail(IFormFile stream, string id);
        public Task<string> UpdateThumbnail(IFormFile stream, string id);

        // Song
        public Task<string> UploadMusic(IFormFile stream, string id);
        public Task<string> GetAuthorisationForFile(string url);
        
        // General
        public Task DeleteResource(string url);
    }
}