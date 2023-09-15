using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("api/admin/genre")]
    public class GenreController : BaseAppController<Genre>
    {
        public GenreController(IResponseHelper response, IBaseService<Genre> service) : base(response, service) { }
    }
}
