
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Models.Db
{
    public class AppDb :DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Band> Bands { get; set; }  
        public DbSet<News>  News { get; set; }
        public DbSet<Playlist>  Playlists { get; set; }
        public DbSet<Role>  Roles { get; set; }
        public DbSet<Song>  Songs { get; set; }
        public DbSet<MemberBand>  Members { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<StatusApplications> StatusApplications { get; set; }
        public DbSet<BlacklistUser> Blacklists { get; set; }
        public DbSet<SocialUserCredential> SocialUserCredentials { get; set; }
        public DbSet<LikedPlaylist> LikedPlaylists { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public AppDb(DbContextOptions<AppDb> options)
            : base(options) => Database.EnsureCreated();
    }
}
