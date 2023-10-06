using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RequestModels;

namespace AdministrationWebApi.Services.DataBase.Interfaces
{
    public interface IAppllicationsService : IBaseService<Application>
    {
        Task<IEnumerable<Application>> GetByUserAsync(Guid id, PaginationInfo pagination);
        Task<IEnumerable<Application>> GetByAdminAsync(Guid id, PaginationInfo pagination);
        Task<IEnumerable<Application>> GetByStatusAsync(Guid id, PaginationInfo pagination);
        Task<Application> ChangeStatusApplicationsAsync(Guid id, CommonRequst entity);
    }
}
