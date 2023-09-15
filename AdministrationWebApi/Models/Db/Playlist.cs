namespace AdministrationWebApi.Models.Db
{
    public class Playlist : Entity
    {     
        public User? User{ get; set; }
        public DateTime CreatePlaylist { get; set; }
        public List<Song> Songs { get; set; } = new();
    }
}
