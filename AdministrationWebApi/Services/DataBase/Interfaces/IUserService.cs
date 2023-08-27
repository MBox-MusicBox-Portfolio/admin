using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationWebApi.Services.DataBase.Interfaces
{
    public interface IUserService: IBaseService<User>
    {
        public Task<bool> ChangeUserRole(Guid id, Role newRole);
        public Task<IEnumerable<User>> GetByRoleAsync(Guid id, PaginationInfo pagination);
    }
}
