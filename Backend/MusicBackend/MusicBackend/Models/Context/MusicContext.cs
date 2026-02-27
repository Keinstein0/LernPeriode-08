using Microsoft.EntityFrameworkCore;
using MusicBackend.Models.DataLayer;
using MusicBackend.Models;

namespace HPlusSportAPI.Models
{
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //1:n
            modelBuilder.Entity<User>()
                .HasMany(user => user.Playlists)
                .WithOne(playlist => playlist.Owner)
                .HasForeignKey(playlist => playlist.OwnerId);

            //N:M
            modelBuilder.Entity<Playlist>()
                .HasMany(playlist => playlist.Songs)
                .WithMany(song => song.playlists);
        }

        public DbSet<User> Users { get; set; } 
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
    }
}
