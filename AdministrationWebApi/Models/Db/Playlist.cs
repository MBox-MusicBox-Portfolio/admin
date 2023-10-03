namespace AdministrationWebApi.Models.Db
{
    public class Playlist : Entity
    {     
        public User? Author{ get; set; }
        public string Name { get; set; }
        public DateTime CreatePlaylist { get; set; }
        public List<Song> Songs { get; set; } = new();
        public bool IsPublic {  get; set; }
        public bool IsUserLibrary { get; set; }
    }
}
