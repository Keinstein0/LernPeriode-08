namespace MusicBackend.Services.BucketInterface
{
    public class MockBucketInterface : IBucketInterface
    {
        async public Task<string> UpdateThumbnail(IFormFile stream, string id)
        {
            return "https://upload.wikimedia.org/wikipedia/commons/f/f9/Phoenicopterus_ruber_in_S%C3%A3o_Paulo_Zoo.jpg";
        }

        async public Task<string> UploadThumbnail(IFormFile stream, string id)
        {
            return "https://upload.wikimedia.org/wikipedia/commons/f/f9/Phoenicopterus_ruber_in_S%C3%A3o_Paulo_Zoo.jpg";
        }


        async public Task<string> GetAuthorisationForFile(string url)
        {
            return "12345";
        }


        async public Task<string> UploadMusic(IFormFile stream, string id)
        {
            return "https://github.com/rafaelreis-hotmart/Audio-Sample-files/raw/master/sample.mp3";
        }

        async public Task DeleteResource(string url)
        {

        }
    }
}
