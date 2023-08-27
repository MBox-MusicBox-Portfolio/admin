using System.ComponentModel.DataAnnotations;

namespace AdministrationWebApi.Models.Db
{
    public class BlacklistUser : Entity
    {
        
        [Required]
        public User? User { get; set; }
        public User? Admin { get; set; }
        public string? Messqge { get; set; }
        public DateTime Created { get; set; }= DateTime.Now;
    }
}
