using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    public class Producer : Entity
    {
        [Required]
        public User User { get; set; }
        [Required]
        [JsonIgnore]
        public List<Band> Bands { get; set; } = new();
    }
}
