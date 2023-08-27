using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    public class StatusApplications : Entity
    {
        
        [Required]
        public string? Name { get; set; }
        [JsonIgnore]
        public List<Applications> Applications { get; set; } = new();
    }
}
