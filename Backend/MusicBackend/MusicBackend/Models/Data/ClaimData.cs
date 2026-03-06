using MusicBackend.Models.Data.User;
using MusicBackend.Models.DataLayer;

namespace MusicBackend.Models.Data
{
    public class ClaimData
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public bool IsSuper {  get; set; }
    }
}
