using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ForAdmin;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("api/admin/users")]
    public class UserController : BaseAppController<User>
    {
        private readonly IUserService _serviceUser;
        private readonly IAdminUserRoleService _adminUserRoleService;
        public UserController(IUserService service, IResponseHelper response, IAdminUserRoleService adminUserRoleService):base(response,service) 
        {
            _serviceUser = service;     
            _adminUserRoleService = adminUserRoleService;
        }

        [HttpGet("pagination")]
        public override Task<ActionResult<ResponsePresenter>> GetAllAsync([FromQuery] PaginationInfo pagination)
        {
            var isSuperAdmin = HttpContext.Items["IsSuperAdmin"] as bool?;
            if (isSuperAdmin.HasValue && isSuperAdmin.Value)
                return base.GetAllAsync(pagination);
            else
                return _adminUserRoleService.GetAllUserAsync(pagination);
        }

        // POST: api/admin/users/{id}/changerole
        [HttpPost("{id}/changerole")]
        public async Task<ActionResult<ResponsePresenter>> ChangeUserRole(Guid id, Role newRole) {
            try
            {
                bool isResult = await _serviceUser.ChangeUserRole(id, newRole);
                return _response.Ok(isResult);
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        // GET: api/admin/users/{id}/role
        [HttpGet("{id}/role")]
        public async Task<ActionResult<ResponsePresenter>> GetByRoleAsync(Guid id, [FromQuery] PaginationInfo pagination)
        {
            try
            {
                IEnumerable<User> items = await _serviceUser.GetByRoleAsync(id, pagination);
                return _response.Ok(GetPresentCollection(items));
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }
    }
}
