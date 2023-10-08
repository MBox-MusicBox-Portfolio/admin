using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    public class Playlist : Entity
    {
        [Required]
        [JsonIgnore]
        public User? Author{ get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Song> Songs { get; set; } = new();
        public bool IsPublic {  get; set; }=false;
    }
}
