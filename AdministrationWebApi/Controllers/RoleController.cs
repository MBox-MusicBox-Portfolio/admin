using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ForAdmin;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationWebApi.Controllers
{
    [Route("api/admin/role")]
    [ApiController]

    public class RoleController : BaseAppController<Role>
    {
        private readonly AdminUserRoleService _adminUserRoleService;
        public RoleController(AdminUserRoleService adminUserRoleService, IResponseHelper response, IBaseService<Role> service) : base(response, service) {
            _adminUserRoleService = adminUserRoleService;
        }

        [HttpGet("pagination")]
        public override Task<ActionResult<ResponsePresenter>> GetAllAsync([FromQuery] PaginationInfo pagination)
        {
            var isSuperAdmin = HttpContext.Items["IsSuperAdmin"] as bool?;
            if (isSuperAdmin.HasValue && isSuperAdmin.Value)
                return base.GetAllAsync(pagination);
            else
                return _adminUserRoleService.GetAllRoleAsync(pagination);
        }
    }
}
