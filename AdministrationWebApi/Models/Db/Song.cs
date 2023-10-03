using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    public class Song : Entity
    {
        
        [Required]
        public string? Name { get; set; }
        public List<Band> Performer { get; } = new();
        public string? Poster { get; set; }
        [Required]
        public string? Link { get; set; }
        public Genre? Genre { get; set; }
        public string? Text { get; set; }
        public string? Description { get; set; }        
        public bool IsBlock { get; set; } = false;
        [JsonIgnore]
        public List<Album> Albums { get; set; } = new();
    }
}
