using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Services.DataBase.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdministrationWebApi.Services.DataBase
{
    public class PlaylistService : BaseService<Playlist>, IByOneMethod<Playlist>
    {
        public PlaylistService(IEntityRepository<Playlist> repository):base(repository) { }

        public async Task<IEnumerable<Playlist>> GetByOneParams(Guid id, PaginationInfo pagination)
        {
            Expression<Func<Playlist, bool>> filter = playlist => playlist.Author.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }
    }
}
