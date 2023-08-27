using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Services.ActionsMailer;
using AdministrationWebApi.Services.DataBase;
using AdministrationWebApi.Services.DataBase.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AdministrationWebApi.Repositories.DataBase
{
    public class UserService : BaseService<User>, IUserService
    {

        private readonly IActionEventRoute _eventRoute;
        private readonly IConfiguration _configuration;
        private readonly IEntityRepository<Role> _repositoryRole;
        public UserService(IEntityRepository<User> repository,
            IActionEventRoute mailer,
            IConfiguration configuration,
            IEntityRepository<Role> repositoryRole)
            : base(repository)
        {

            _eventRoute = mailer;
            _configuration = configuration;
            _repositoryRole = repositoryRole;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var user = await GetUserByidAsync(id);
            if (user == null)
            {
                return false;
            }

            var result = await base.DeleteAsync(id);
            if (result)
            {
                _ = _eventRoute.UserAction(user, _configuration["TemplatePages:USER_DELETE"]);
            }
            return result;
        }

        public override async Task<User> UpdateAsync(User entity)
        {
            var user = await GetUserByidAsync(entity.Id);
            var userUp = await base.UpdateAsync(entity);
            if (user.Role != userUp.Role) {
                await _eventRoute.UserAction(userUp, _configuration["TemplatePages:USER_CHANGE_ROLE"]);
            }
            return userUp;
        }

        public async Task<bool> ChangeUserRole(Guid id, Role newRole)
        {
            var user = await GetByIdAsync(id);
            if (user == null)
            {
                throw new NotFoundException("Not found User", "User");
            }
            var role = await _repositoryRole.GetByIdAsync(newRole.Id);
            if (role == null)
            {
                throw new NotFoundException("Not found Role for User", "User");
            }
            user.Role = role;
            await UpdateAsync(user);
            await _eventRoute.UserAction(user, _configuration["TemplatePages:USER_CHANGE_ROLE"]);
            return true;
        }

        public async Task<IEnumerable<User>> GetByRoleAsync(Guid id, PaginationInfo pagination)
        {
            Expression<Func<User, bool>> filter = app => app.Role.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }

        private async Task<User?> GetUserByidAsync(Guid id)
        {
            return await BuildQuery().FirstOrDefaultAsync(x => x.Id == id);
        }

    }

}
