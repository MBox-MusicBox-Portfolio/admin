using System.ComponentModel.DataAnnotations;

namespace AdministrationWebApi.Models.Db
{
    public class SocialUserCredential : Entity
    {
        [Required]
        public User? User { get; set; }        
        public string? SocialNetwork { get; set; }
        public string? SocialUserID { get; set; }
    }
}
