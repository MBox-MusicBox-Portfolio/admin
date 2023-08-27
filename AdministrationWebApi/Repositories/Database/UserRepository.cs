using AdministrationWebApi.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Repositories.Database
{
    public class UserRepository : BaseRepository<User>
    {
        public UserRepository(AppDb dbContext) : base(dbContext) { }
        public override IQueryable<User> BuildQuery()
        {
            return _context.Users.Include(u => u.Role);
        }
    }
}
