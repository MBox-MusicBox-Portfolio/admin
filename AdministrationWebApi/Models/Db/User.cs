using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdministrationWebApi.Models.Db
{
    [Index(nameof(Email), IsUnique = true)]
    public class User : Entity
    {
        
        [Required]
        public string Name { get; set; }
        public string? Avatar { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public DateTime? Birthday { get; set; }
        [Required]
        public Role? Role { get; set; }
        [JsonIgnore]
        public List<Band>? FollowingsBands { get; set; } = new();
        [Required]
        [JsonIgnore]
        public string? Password { get; set; }
        public bool IsEmailVerify { get; set; }=false;
        public bool IsBlocked { get; set;}=false;
    }
}
