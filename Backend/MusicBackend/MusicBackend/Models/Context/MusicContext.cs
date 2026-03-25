using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MusicBackend.Models;
using MusicBackend.Models.DataLayer;

namespace MusicBackend.Models
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
                .OnDelete(DeleteBehavior.Cascade)
                .HasForeignKey(playlist => playlist.OwnerId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username) //unique username
                .IsUnique();

            //N:M from Playlist to Song
            modelBuilder.Entity<Playlist>()
                .HasMany(e => e.Songs)
                .WithMany(e => e.Playlists)
                .UsingEntity<PlaylistSong>(
                    r => r.HasOne<Song>(e => e.Song).WithMany(e => e.PlaylistSongs).OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<Playlist>(e => e.Playlist).WithMany(e => e.PlaylistSongs).OnDelete(DeleteBehavior.Cascade));
        }

        public DbSet<User> Users { get; set; } 
        public DbSet<Song> Songs { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistSong> PlaylistSongs { get; set; }
    }
}
