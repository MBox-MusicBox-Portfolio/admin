using AdministrationWebApi.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Repositories.Database
{
    public class GenreRepository : BaseRepository<Genre>
    {
        public GenreRepository(AppDb dbContexto) : base(dbContexto) { }        
    }
}
