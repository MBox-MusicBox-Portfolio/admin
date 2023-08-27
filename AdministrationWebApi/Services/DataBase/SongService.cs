using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Models.Exceptions;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Services.ActionsMailer;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Services.DataBase
{
    public class SongService : BaseService<Song>, ISongService
    {

        private readonly IActionEventRoute _eventRoute;
        private readonly IConfiguration _configuration;

        public SongService(IEntityRepository<Song> repository, IActionEventRoute mailer, IConfiguration configuration, IResponseHelper response) : base(repository)
        {
            _eventRoute = mailer;
            _configuration = configuration;

        }
        public override async Task<bool> DeleteAsync(Guid id)
        {
            var song = await GetSongByIdAsync(id);
            if (song == null)
            {
                return false;
            }

            var result = await base.DeleteAsync(id);
            if (result)
            {
                _ = _eventRoute.SongAction(song, _configuration["TemplatePages:DELETE_SONG"]);
            }
            return result;
        }
        public async Task<bool> BlockSong(Guid id)
        {
            var song = await GetSongByIdAsync(id);

            if (song == null)
            {
                throw new NotFoundException(" Not found Song", "Song");
            }
            song.IsBlock = true;
            await UpdateAsync(song);
            _ = _eventRoute.SongAction(song, _configuration["TemplatePages:BLOCK_SONG"]);
            return true;
        }

        public async Task<bool> UnBlockSong(Guid id)
        {
            var song = await GetSongByIdAsync(id);

            if (song == null)
            {
                throw new NotFoundException(" Not found Song", "Song");
            }
            song.IsBlock = false;
            await UpdateAsync(song);
            _ = _eventRoute.SongAction(song, _configuration["TemplatePages:UNBLOCK_SONG"]);
            return true;
        }
        private async Task<Song?> GetSongByIdAsync(Guid id)
        {
            return await BuildQuery(song => song.Id == id)
                 .Include(song => song.Performer)
                 .FirstOrDefaultAsync();
        }
    }
}
