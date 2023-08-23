using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.RabbitMq;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Services.RabbitMQ;
using AdministrationWebApi.Services.SignalR;
using Microsoft.AspNetCore.SignalR;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Repositories.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Services.ResponseHelper;
using AdministrationWebApi.Services.DataBase;
using System.Linq.Expressions;

namespace AdministrationWebApi.Repositories.DataBase
{
    public class ApplicationService : BaseService<Applications>, IAppllicationsService
    {

        private readonly IConfiguration _configuration;
        private readonly RabbitMqService _rabbit;
        private readonly IHubContext<NotificationSignalR> _hubContext;
        private readonly IEntityRepository<StatusApplications> _repositoryStatus;
        private readonly IEntityRepository<User> _repositoryUser;
        private readonly IEntityRepository<Band> _repositoryBand;
        private readonly IEntityRepository<Producer> _repositoryProducer;
        private readonly IEntityRepository<Role> _repositoryRole;


        public ApplicationService(RabbitMqService rabbit,
            IConfiguration configuration,
            IHubContext<NotificationSignalR> hubContext,
            IEntityRepository<Applications> app,
            IEntityRepository<StatusApplications> repositoryStatus,
            IEntityRepository<User> repositoryUser,
            IEntityRepository<Band> repositoryBand,
            IEntityRepository<Producer> repositoryProducer,
            IEntityRepository<Role> repositoryRole)
            : base(app)
        {
            _rabbit = rabbit;
            _hubContext = hubContext;
            _configuration = configuration;
            _repositoryStatus = repositoryStatus;
            _repositoryUser = repositoryUser;
            _repositoryBand = repositoryBand;
            _repositoryProducer = repositoryProducer;
            _repositoryRole = repositoryRole;
        }

        public async Task<Applications> ChangeStatusApplicationsAsync(Guid id, CommonRequst entity)
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
            var admin = await _repositoryUser.BuildQuery().FirstOrDefaultAsync(u => u.Id == entity.IdAdmin && (u.Role.Name == "admin" || u.Role.Name == "super_admin"));
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
                if (newStatus.Name == "accepted")
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

            //var msg = new SendObject()
            //{
            //    Email = application.Producer?.Email,
            //    Template = "application_change_status_mail", //change_status_application_mail
            //    Name = application.Producer?.Name,
            //    Body = new { Status = application.Status.Name }
            //};
            //_rabbit.SendMessage(msg, _configuration["Queue:MAILER"]);
            await _hubContext.Clients.Group(application.Producer.Id.ToString()).SendAsync("ReceiveMessage", "change_status_applicaction");
            return application;
        }

        public async Task<IEnumerable<Applications>> GetByAdminAsync(Guid id, PaginationInfo pagination)
        {
            Expression<Func<Applications, bool>> filter = app => app.Admin.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }

        public async Task<IEnumerable<Applications>> GetByStatusAsync(Guid id, PaginationInfo pagination)
        {
            Expression<Func<Applications, bool>> filter = app => app.Status.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }

        public async Task<IEnumerable<Applications>> GetByUserAsync(Guid id, PaginationInfo pagination)
        {
            Expression<Func<Applications, bool>> filter = app => app.Producer.Id == id;
            return await BuildQuery(filter, pagination).ToListAsync();
        }

        private Band CreateBandWithApplication(Applications app)
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
