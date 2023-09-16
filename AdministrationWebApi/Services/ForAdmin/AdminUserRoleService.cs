using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdministrationWebApi.Services.ForAdmin
{
    public class AdminUserRoleService : IAdminUserRoleService
    {
        private readonly IBaseService<Role> _roleService;
        private readonly IUserService _userService;
        protected readonly IResponseHelper _response;
        public AdminUserRoleService(IBaseService<Role> roleService, IUserService userService, IResponseHelper response)
        {
            _roleService = roleService;
            _userService = userService;
            _response = response;
        }
        public async Task<ActionResult<ResponsePresenter>> GetAllRoleAsync(PaginationInfo pagination)
        {
            Expression<Func<Role, bool>> filter = role => role.Name != "admin" && role.Name != "super_admin";
            var result = await _roleService.BuildQuery(filter, pagination).ToListAsync();
            return _response.Ok(result);
        }

        public async Task<ActionResult<ResponsePresenter>> GetAllUserAsync(PaginationInfo pagination)
        {
            Expression<Func<User, bool>> filter = user => user.Role != null && user.Role.Name != "admin" && user.Role.Name != "super_admin";
            var result = await _userService.BuildQuery(filter, pagination).ToListAsync();
            return _response.Ok(result);
        }
    }
}
