using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Repositories.DataBase.Interfaces;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AdministrationWebApi.Controllers
{
    [ApiController]
    [Route("api/admin/bands")]
    public class BandsController : BaseAppController<Band>
    {
        private readonly IBandService _serviceBand;
        public BandsController(IBandService service, IResponseHelper response):base(response, service)
        {
            _serviceBand = service;          
        }


        [HttpPost("{id}/members")]
        public async Task<ActionResult<ResponsePresenter>> AddMemberToBand(Guid id, [FromBody] Guid member)
        {
            try
            {
                var item = await _serviceBand.AddMemberToBandAsync(id, member);
                return _response.Ok(item);
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        [HttpPost("{id}/blacklist")]
        public async Task<ActionResult<ResponsePresenter>> Block(Guid id)
        {
            try
            {
                var item = await _serviceBand.BlockBandAsync(id);
                return _response.Ok(item);
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }

        [HttpDelete("{id}/removefromblacklist")]
        public async Task<ActionResult<ResponsePresenter>> Unblock(Guid id)
        {
            try
            {
                var item = await _serviceBand.UnblockAsync(id);
                return _response.Ok(item);
            }
            catch (Exception ex)
            {
                return _response.HandleError(ex);
            }
        }
        protected override object GetPresent(Band entity)
        {
            return new BandsPresent(entity);
        }
    }
}
