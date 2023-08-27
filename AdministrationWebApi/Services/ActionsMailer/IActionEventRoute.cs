using AdministrationWebApi.Models.Db;

namespace AdministrationWebApi.Services.ActionsMailer
{
    public interface IActionEventRoute
    {
        Task NewsDelete(News news);
        Task SongAction(Song song, string? template);
        Task BandAction(Band band, string? template);
        Task UserAction(User user, string? template);
    }
}
