using AdministrationWebApi.Controllers;
using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/applications")]
public class ApplicationsController : BaseAppController<Application>
{
    private readonly IAppllicationsService _serviceApp;    
    public ApplicationsController(IAppllicationsService service, IResponseHelper response):base(response,service)
    {
        _serviceApp = service;  
    }


    // PUT: api/admin/applications/{id}/changestatus
    [HttpPut("{id}/changestatus")]
    public async Task<ActionResult<ResponsePresenter>> UpdateApplication(Guid id, CommonRequst status)
    {
        try
        {
            Application item = await _serviceApp.ChangeStatusApplicationsAsync(id, status);
            return _response.Ok(new ApplicationPresenter(item));
        }
        catch (Exception ex)
        {
            return _response.HandleError(ex);
        }
    }

    // GET: api/admin/applications/{id}/user
    [HttpGet("{id}/user/{pagination}")]
    public async Task<ActionResult<ResponsePresenter>> GetApplicationsByUser(Guid id, [FromQuery] PaginationInfo pagination)
    {
        try
        {
            IEnumerable<Application> items = await _serviceApp.GetByUserAsync(id, pagination);
            return _response.Ok(GetPresentCollection(items));
        }
        catch (Exception ex)
        {
            return _response.HandleError(ex);
        }
    }

    // GET: api/admin/applications/{id}/status
    [HttpGet("{id}/status/{pagination}")]
    public async Task<ActionResult<ResponsePresenter>> GetApplicationsByStatus(Guid id, [FromQuery] PaginationInfo pagination)
    {
        try
        {
            IEnumerable<Application> items =  await _serviceApp.GetByStatusAsync(id, pagination);
            return _response.Ok(GetPresentCollection(items));
        }
        catch (Exception ex)
        {
            return _response.HandleError(ex);
        }
    }

    // GET: api/admin/applications/{id}/admin/{pagination}
    [HttpGet("{id}/admin/{pagination}")]
    public async Task<ActionResult<ResponsePresenter>> GetApplicationsByAdmin(Guid id, [FromQuery] PaginationInfo pagination)
    {
        try
        {
            IEnumerable<Application> items = await _serviceApp.GetByAdminAsync(id, pagination);
            return _response.Ok(GetPresentCollection(items));
        }     
        catch (Exception ex)
        {
            return _response.HandleError(ex);
        }
    }

    protected override object GetPresent(Application entity)
    {
        return new ApplicationPresenter(entity);
    }
}
