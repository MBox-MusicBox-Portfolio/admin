using System.ComponentModel.DataAnnotations;

namespace AdministrationWebApi.Models.Db
{
    public class LikedPlaylist:Entity
    {
        [Required]
        public User? User { get; set; }
        [Required]
        public Playlist? Playlist { get; set; }
    }
}
