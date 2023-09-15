using System.ComponentModel.DataAnnotations;

namespace AdministrationWebApi.Models.Db
{
    public class Genre:Entity
    {
        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }
    }
}
