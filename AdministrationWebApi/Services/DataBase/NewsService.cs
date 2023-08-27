using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Models.RequestModels;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Repositories.DataBase.Interfaces;
using AdministrationWebApi.Services.ActionsMailer;
using AdministrationWebApi.Services.DataBase;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Repositories.DataBase
{
    public class NewsService : BaseService<News>, INewsService
    {

        private readonly IActionEventRoute _eventRoute;
        public NewsService(IEntityRepository<News> repository,
            IActionEventRoute eventRoute)
            : base(repository)
        {
            _eventRoute = eventRoute;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var news = await BuildQuery(news => news.Id == id)
                    .Include(news => news.Member)
                    .FirstOrDefaultAsync();
            if (news == null)
            {
                return false;
            }

            var result = await base.DeleteAsync(id);

            if (result)
            {
                _ = _eventRoute.NewsDelete(news);
            }

            return result;
        }

        public async Task<IEnumerable<News>> GetNewsByAuthor(Guid authorId, PaginationInfo pagination)
        {
            var news = await BuildQuery(n => n.Author != null && n.Author.Id == authorId, pagination).ToListAsync();

            if (news == null || news.Count == 0)
            {
                throw new NotFoundException("Not found News", "News");
            }

            return news;
        }

        public async Task<IEnumerable<News>> GetNewsByBand(Guid bandId, PaginationInfo pagination)
        {
            var news = await BuildQuery(n => n.Author != null && n.Author.Id == bandId, pagination).ToListAsync();

            if (news == null || news.Count == 0)
            {
                throw new NotFoundException("Not found News", "News");
            }

            return news;
        }
    }
}
