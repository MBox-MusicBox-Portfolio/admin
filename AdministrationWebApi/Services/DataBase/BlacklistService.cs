using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Repositories.DataBase.Interfaces;
using AdministrationWebApi.Services.ActionsMailer;
using AdministrationWebApi.Services.DataBase;
using AdministrationWebApi.Services.ResponseHelper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Repositories.DataBase
{
    public class BlacklistService : BaseService<BlacklistUser>, IBlacklistService
    {

        private readonly IConfiguration _configuration;
        private readonly IActionEventRoute _eventRoute;     
        private readonly IEntityRepository<User> _repositoryUser;


        public BlacklistService(IEntityRepository<BlacklistUser> repository,
            IConfiguration config,
            IActionEventRoute rabbit,            
            IEntityRepository<User> repositoryUser)
            : base(repository)
        {
            _configuration = config;
            _eventRoute = rabbit;
            _repositoryUser = repositoryUser;
        }

        public async Task<bool> BlockUserAsync(Guid id, CommonRequst entity)
        {
            var user = await _repositoryUser.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException(ResponseHelper.CreateObjectError("Blacklist", "Not found user"));
            }
            if (user.IsBlocked)
            {
                throw new NotFoundException(ResponseHelper.CreateObjectError("Blacklist", "The user is already blocked"));
            }
            var admin = await _repositoryUser.BuildQuery().FirstOrDefaultAsync(user => user.Id == entity.IdAdmin && (user.Role.Name == "admin" || user.Role.Name == "super_admin"));
            if (admin == null)
            {
                throw new NotFoundException(ResponseHelper.CreateObjectError("Blacklist", "The admin is bad"));
            }
            user.IsBlocked = true;
            var item = new BlacklistUser()
            {
                User = user,
                Admin = admin,
                Messqge = entity.Message
            };
            await AddAsync(item);
            await _eventRoute.UserAction(item.User, _configuration["TemplatePages:USER_BLOCK"]);           
            return true;
        }

        public async Task<bool> UnblockUserAsync(Guid id)
        {
            var user = await _repositoryUser.GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException(ResponseHelper.CreateObjectError("Blacklist", "Not found user"));
            }
            if (!user.IsBlocked)
            {
                throw new NotFoundException(ResponseHelper.CreateObjectError("Blacklist", "The user is already unblocked"));
            }
            user.IsBlocked = false;
            await _repositoryUser.UpdateAsync(user);
            await _eventRoute.UserAction(user, _configuration["TemplatePages:USER_UNBLOCK"]);
            return true;
        }

        public async Task<IEnumerable<BlacklistUser>> GetByUserAsync(Guid id, PaginationInfo pagination)
        {
            var history = await BuildQuery(x => x.User != null && x.User.Id == id, pagination).ToListAsync();
            return history;
        }
        public async Task<IEnumerable<BlacklistUser>> GetByAdminAsync(Guid id, PaginationInfo pagination)
        {
            var history = await BuildQuery(x => x.Admin != null && x.Admin.Id == id, pagination).ToListAsync();
            return history;
        }
    }
}
