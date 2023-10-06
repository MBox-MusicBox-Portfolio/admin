using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.RabbitMQ;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Repositories.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Services.ResponseHelper;
using AdministrationWebApi.Services.DataBase;
using System.Linq.Expressions;
using AdministrationWebApi.Models.RabbitMq;

namespace AdministrationWebApi.Repositories.DataBase
{
    public class ApplicationService : BaseService<Application>, IAppllicationsService
    {

        private readonly IConfiguration _configuration;
        private readonly RabbitMqService _rabbit;
        private readonly IEntityRepository<StatusApplications> _repositoryStatus;
        private readonly IEntityRepository<User> _repositoryUser;
        private readonly IEntityRepository<Band> _repositoryBand;
        private readonly IEntityRepository<Producer> _repositoryProducer;
        private readonly IEntityRepository<Role> _repositoryRole;


        public ApplicationService(RabbitMqService rabbit,
            IConfiguration configuration,
            IEntityRepository<Application> app,
            IEntityRepository<StatusApplications> repositoryStatus,
            IEntityRepository<User> repositoryUser,
            IEntityRepository<Band> repositoryBand,
            IEntityRepository<Producer> repositoryProducer,
            IEntityRepository<Role> repositoryRole)
            : base(app)
        {
            _rabbit = rabbit;
            _configuration = configuration;
            _repositoryStatus = repositoryStatus;
            _repositoryUser = repositoryUser;
            _repositoryBand = repositoryBand;
            _repositoryProducer = repositoryProducer;
            _repositoryRole = repositoryRole;
        }

        public async Task<Application> ChangeStatusApplicationsAsync(Guid id, CommonRequst entity)
        {
            var application = await BuildQuery().FirstOrDefaultAsync(app => app.Id == id);
            List<object> errors = new();
            if (application == null)
            {
                errors.Add(ResponseHelper.CreateObjectError("Id", "Not found Application"));
            }
            var newStatus = await _repositoryStatus.GetByIdAsync(entity.IdObject);
            if (newStatus == null)
            {
                errors.Add(ResponseHelper.CreateObjectError("Status", "Not found Status"));
            }
            var admin = await _repositoryUser.BuildQuery().FirstOrDefaultAsync(u => u.Id == entity.IdAdmin && (u.Role != null && u.Role.Name == "admin" || u.Role.Name == "super_admin"));
            if (admin == null)
            {
                errors.Add(ResponseHelper.CreateObjectError("Admin", "Not found Admin"));
            }
            if (errors.Count > 0)
            {
                throw new NotFoundException(errors);
            }
            try
            {
                application.Status = newStatus;
                application.Admin = admin;
                application.MessageCreated = entity.Message;
                application.ChangedStatus = DateTime.Now;
                if (newStatus?.Name == "accepted")
                {
                    var band = CreateBandWithApplication(application);
                    await _repositoryBand.AddAsync(band);
                }
                await UpdateAsync(application);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new BadRequestException("The server error occurred");
            }

            var msg = new EventRoute()
            {
                To = application.Producer?.Id.ToString(),
                Template = "application_change_status_mail",
                Body = new { Email = application.Producer?.Email, Status = application.Status?.Name, Name = application.Producer?.Name }
            };
            _rabbit.SendMessage(msg);
            return application;
        }

        public async Task<IEnumerable<Application>> GetByAdminAsync(Guid id, PaginationInfo pagination)
        {
            Expression<Func<Application, bool>> filter = app => app.Admin != null && app.Admin.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByStatusAsync(Guid id, PaginationInfo pagination)
        {
            Expression<Func<Application, bool>> filter = app => app.Status != null && app.Status.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }

        public async Task<IEnumerable<Application>> GetByUserAsync(Guid id, PaginationInfo pagination)
        {
            Expression<Func<Application, bool>> filter = app => app.Producer != null && app.Producer.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }

        private Band CreateBandWithApplication(Application app)
        {
            User? user = app.Producer;
            var producer = _repositoryProducer.GetByIdAsync(user.Id).Result;
            if (producer == null)
            {
                var role = _repositoryRole.BuildQuery().FirstOrDefault(r => r.Name == "producer");
                if (role == null)
                {
                    throw new NotFoundException(ResponseHelper.CreateObjectError("Application", "Cannot find matching roles"));
                }
                user.Role = role;
            }

            var band = new Band()
            {
                Name = app.BandName,
                Producer = producer ?? new Producer() { User = user },
                FullInfo = app.FullInfo
            };
            return band;
        }
    }
}
