using AdministrationWebApi.Models.Db;
using System.ComponentModel.DataAnnotations;

namespace AdministrationWebApi.Models.Presenter
{
    public class ApplicationPresenter
    {
        public Guid Id { get; set; } = Guid.NewGuid();   
        public string? BandName { get; set; }
        public string? FullInfo { get; set; }        
        public UserPresenter? Producer { get; set; }
        public UserPresenter? Admin { get; set; }
        public string? MessageCreated { get; set; }
        public CommonPresenter? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ChangedStatus { get; set; }
        public ApplicationPresenter(Application applications)
        {
            Id= applications.Id;
            BandName= applications.BandName;
            FullInfo= applications.FullInfo;
            Producer= applications.Producer != null ? new UserPresenter(applications.Producer) : null;
            Admin = applications.Admin != null ? new UserPresenter(applications.Admin) : null;
            MessageCreated =applications.MessageCreated;
            Status= applications.Status != null?new CommonPresenter(applications.Status):null;
            CreatedAt=applications.CreatedAt;
            ChangedStatus=applications.ChangedStatus;
        }
    }
}
