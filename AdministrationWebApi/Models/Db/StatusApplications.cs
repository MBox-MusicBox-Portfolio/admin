using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    public class StatusApplications : Entity
    {
        
        [Required]
        public string? Name { get; set; }
        [JsonIgnore]
        public List<Application> Applications { get; set; } = new();
    }
}
