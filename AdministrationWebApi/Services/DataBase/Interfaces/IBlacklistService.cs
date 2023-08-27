using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.DataBase.Interfaces;

namespace AdministrationWebApi.Repositories.DataBase.Interfaces
{
    public interface IBlacklistService : IBaseService<BlacklistUser>
    {
        Task<bool> BlockUserAsync(Guid id, CommonRequst requst);
        Task<bool> UnblockUserAsync(Guid id);
        Task<IEnumerable<BlacklistUser>> GetByAdminAsync(Guid id, PaginationInfo pagination);
        Task<IEnumerable<BlacklistUser>> GetByUserAsync(Guid id, PaginationInfo pagination);
    }
}
