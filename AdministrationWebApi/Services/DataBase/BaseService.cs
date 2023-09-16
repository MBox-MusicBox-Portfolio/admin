using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Services.DataBase.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdministrationWebApi.Services.DataBase
{
    public class BaseService<TEntity> : IBaseService<TEntity>
    {
        protected IEntityRepository<TEntity> _repository;
        public BaseService(IEntityRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            return await _repository.AddAsync(entity);
        }

        public IQueryable<TEntity> BuildQuery()
        {
            return _repository.BuildQuery();
        }
        public IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>> filter, PaginationInfo pagination)
        {
            var applications = BuildQuery()
                .Where(filter)
                .Skip((pagination.Page - 1) * pagination.Size)
                .Take(pagination.Size);

            return applications;
        }
      
        public IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>> filter)
        {
            var applications = BuildQuery()
                .Where(filter);

            return applications;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            return await _repository.DeleteAsync(id);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(PaginationInfo pagination)
        {
            return await _repository.GetAllAsync(pagination);
        }

        public virtual async Task<TEntity> GetByIdAsync(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await _repository.UpdateAsync(entity);
        }
    }
}
