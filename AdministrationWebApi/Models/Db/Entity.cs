using System.ComponentModel.DataAnnotations;

namespace AdministrationWebApi.Models.Db
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
