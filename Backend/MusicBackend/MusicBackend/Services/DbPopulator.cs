using Microsoft.EntityFrameworkCore;
using MusicBackend.Models;
using MusicBackend.Models.DataLayer;

namespace MusicBackend.Services
{
    public static class DbPopulator
    {
        public static async Task InitializeAsync(MusicContext context)
        {
            // Ensures the DB schema is created
            await context.Database.EnsureCreatedAsync();

            string username = Environment.GetEnvironmentVariable("SUPERUSER_NAME");
            string password = Environment.GetEnvironmentVariable("SUPERUSER_PWD");

            if (username == null || password == null)
            {
                throw new ArgumentNullException("username or pwd not found in .env");
            }

            // Check if admin exists
            if (!await context.Users.AnyAsync<User>(u => u.Username == username))
            {
                var admin = new User
                {
                    Username = username,
                    Hash = BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 13),
                    Id = Guid.NewGuid().ToString(),
                    IsSuper = true,
                };

                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
        }
    }
}
