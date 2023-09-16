using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Models.Presenter;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Repositories.DataBase.Interfaces;
using AdministrationWebApi.Services.ActionsMailer;
using AdministrationWebApi.Services.DataBase;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AdministrationWebApi.Repositories.DataBase
{
    public class BandService : BaseService<Band>, IBandService
    {
        private readonly IActionEventRoute _eventRoute;
        private readonly IConfiguration _configuration;      
        private readonly IEntityRepository<User> _repositoryUser;
        private readonly IEntityRepository<MemberBand> _repositoryMember;
        private readonly IEntityRepository<Role> _repositoryRole;
        public BandService(IActionEventRoute mailer,
            IConfiguration configuration,
            IResponseHelper response,
            IEntityRepository<Band> repository,
            IEntityRepository<User> repositoryUser,
            IEntityRepository<MemberBand> repositoryMember,
            IEntityRepository<Role> repositoryRole)
            : base(repository)
        {
            _eventRoute = mailer;
            _configuration = configuration;            
            _repositoryUser = repositoryUser;
            _repositoryMember = repositoryMember;
            _repositoryRole = repositoryRole;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var band = await GetByIdAsync(id);
            var isResult = await base.DeleteAsync(id);
            await _eventRoute.BandAction(band, _configuration["TemplatePages:DELETE_BAND"]);
            return isResult;
        }

        public async Task<Band> AddMemberToBandAsync(Guid id, Guid memberId)
        {
            var band = await BuildQuery().Include(b => b.Members).FirstOrDefaultAsync(b => b.Id == id);
            if (band == null)
            {
                throw new NotFoundException("Not found Band", "Band");
            }

            var tmpMember = band.Members.FirstOrDefault(m => m.Id == memberId);
            if (tmpMember == null)
            {
                tmpMember = await _repositoryMember.GetByIdAsync(memberId);
                if (tmpMember == null)
                {
                    var newMember = await _repositoryUser.BuildQuery().FirstOrDefaultAsync(u => u.Id == memberId);
                    if (newMember == null)
                    {
                        throw new NotFoundException("Not found User", "Band");
                    }
                    var role = await _repositoryRole.BuildQuery().FirstOrDefaultAsync(u => u.Name == "musician");
                    if (role == null)
                    {
                        throw new NotFoundException("Not found Role 'musician'", "Band");
                    }
                    newMember.Role = role;
                    tmpMember = new MemberBand()
                    {
                        User = newMember
                    };
                    await _eventRoute.UserAction(newMember, _configuration["TemplatePages: USER_CHANGE_ROLE"]);
                    await _repositoryMember.AddAsync(tmpMember);
                }
                band.Members.Add(tmpMember);
                await UpdateAsync(band);
                return band;
            }

            throw new BadRequestException("The user is already a member of the band", "Band");
        }

        public async Task<bool> BlockBandAsync(Guid id)
        {
            var band = await GetByIdAsync(id);

            if (band == null)
            {
                throw new NotFoundException("Not found Band", "Band");
            }
            if (band.IsBlocked)
            {
                throw new NotFoundException("The band is already in the blacklist.", "Band");
            }

            band.IsBlocked = true;
            await UpdateAsync(band);
            _ = _eventRoute.BandAction(band, _configuration["TemplatePages:BLOCK_BAND"]);
            return band.IsBlocked;
        }

        public async Task<bool> UnblockAsync(Guid id)
        {
            var band = await GetByIdAsync(id);
            if (band == null)
            {
                throw new NotFoundException("Not found Band", "Band");
            }
            if (!band.IsBlocked)
            {
                throw new NotFoundException("The band is not in the blacklist.", "Band");
            }
            band.IsBlocked = false;
            await UpdateAsync(band);
            await _eventRoute.BandAction(band, _configuration["TemplatePages:UNBLOCK_BAND"]);
            return band.IsBlocked;
        }


    }
}

