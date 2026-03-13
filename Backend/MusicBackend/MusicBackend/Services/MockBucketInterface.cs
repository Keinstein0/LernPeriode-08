
namespace MusicBackend.Services
{
    public class MockBucketInterface : IBucketInterface
    {
        public Task UploadMusic()
        {
            return null;
        }

        public Task UploadThumbnail(FileStream thumbnail)
        {
            return null;
        }
    }
}
