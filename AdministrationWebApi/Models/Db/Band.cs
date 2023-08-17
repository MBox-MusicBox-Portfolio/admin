using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    public class Band
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Avatar { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public Producer? Producer { get; set; }
        [JsonIgnore]
        public List<MemberBand> Members { get; set; } = new();
        [JsonIgnore]
        public List<Song> Songs { get; set; } = new();
        [JsonIgnore]
        public List<Album> Albums { get; set; } = new();
        public DateTime? CreatedAt { get; set; }=DateTime.Now;
        public string? FullInfo { get; set; }
        [JsonIgnore]
        public List<User>? Followers {get; set; } = new();
        public bool IsBlocked { get; set; } = false;
    }
}
