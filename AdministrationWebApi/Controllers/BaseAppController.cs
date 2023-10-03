using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    public class BaseAppController<TEntity> : ControllerBase
    {
        protected readonly IResponseHelper _response;
        protected readonly IBaseService<TEntity> _service;
        public BaseAppController(IResponseHelper response, IBaseService<TEntity> service)
        {
            _response = response;
            _service = service;
        }

        [HttpGet("pagination")]
        public virtual async Task<ActionResult<ResponsePresenter>> GetAllAsync([FromQuery] PaginationInfo pagination)
        {
            try
            {
                IEnumerable<TEntity> items = await _service.GetAllAsync(pagination);
                return _response.Ok(GetPresentCollection(items));
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResponsePresenter>> GetByIdAsync(Guid id)
        {
            try
            {
                TEntity item = await _service.GetByIdAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(GetPresent(item));
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponsePresenter>> CreateAsync([FromBody] TEntity entity)
        {
            try
            {
                TEntity createdEntity = await _service.AddAsync(entity);
                return _response.Created(GetPresent(createdEntity));
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ResponsePresenter>> UpdateAsync([FromBody] TEntity entity)
        {
            try
            {                
                await _service.UpdateAsync(entity);
                return _response.NoContent();
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ResponsePresenter>> DeleteAsync(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return _response.NoContent();
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        protected virtual object GetPresent(TEntity entity) => entity;
        protected virtual IEnumerable<object> GetPresentCollection(IEnumerable<TEntity>  entity)
        {
            return entity.Select(obj => GetPresent(obj));
        }
    }
}
