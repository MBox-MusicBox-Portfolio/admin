using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RequestModels;
using System.Linq.Expressions;

namespace AdministrationWebApi.Repositories.Database.Interfaces
{
    public interface IEntityRepository<TEntity>
    {
        Task<TEntity> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync(PaginationInfo pagination);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(Guid id);
        IQueryable<TEntity> BuildQuery();
     
    }
}
