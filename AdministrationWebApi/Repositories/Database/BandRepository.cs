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
                .Include(band => band.Producer)
                .Include(band => band.Songs)
                .Include(band => band.Followers)
                .Include(band => band.Members)
                .Include(band => band.Albums)
                .Include(band => band.Genres);
        }
    }
}
