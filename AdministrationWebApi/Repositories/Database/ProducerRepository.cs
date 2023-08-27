using AdministrationWebApi.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Repositories.Database
{
    public class ProducerRepository:BaseRepository<Producer>
    {
        public ProducerRepository(AppDb  dbContext) : base(dbContext) { }
        public override IQueryable<Producer> BuildQuery()
        {
            return _context.Producers.Include(u => u.User);
        }
    }
}
