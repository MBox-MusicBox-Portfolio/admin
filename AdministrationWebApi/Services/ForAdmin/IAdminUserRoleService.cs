using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationWebApi.Services.ForAdmin
{
    public interface IAdminUserRoleService
    {
        public Task<ActionResult<ResponsePresenter>> GetAllRoleAsync(PaginationInfo pagination);
        public Task<ActionResult<ResponsePresenter>> GetAllUserAsync(PaginationInfo pagination);

    }
}
