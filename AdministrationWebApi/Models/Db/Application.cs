using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    public class Application:Entity
    {
      
        [Required]
        public string? BandName { get; set; }
        public string? FullInfo { get; set; }
        [JsonIgnore]
        public User? Producer { get; set; }
        [JsonIgnore]
        public User? Admin { get; set; }
        public string? MessageCreated { get; set; }
        public StatusApplications? Status { get; set; }        
        public DateTime CreatedAt { get; set; }= DateTime.Now;       
        public DateTime? ChangedStatus { get; set; }
    }
}
