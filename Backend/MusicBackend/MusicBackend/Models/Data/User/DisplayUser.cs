namespace MusicBackend.Models.Data.User
{
    public class DisplayUser
    {
        public required string Id { get; set; }
        public required string Username { get; set; }
        public bool IsSuper {  get; set; }

        public static void ToDisplayUsers(List<MusicBackend.Models.DataLayer.User> users, out List<DisplayUser> displayUsers)
        {
            displayUsers = new List<DisplayUser>();

            foreach (var user in users)
            {
                displayUsers.Add(user.AsDisplayUser());
            }
        }
    }
}
