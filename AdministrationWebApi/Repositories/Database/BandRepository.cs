using AdministrationWebApi.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Repositories.Database
{
    public class BandRepository:BaseRepository<Band>
    {
        public BandRepository(AppDb dbContexto) : base(dbContexto) { }
        public override IQueryable<Band> BuildQuery()
        {
            return _context.Bands
                .Include(app => app.Producer)
                .Include(app => app.Songs)
                .Include(app => app.Followers)
                .Include(app => app.Members)
                .Include(app => app.Albums);
        }
    }
}
