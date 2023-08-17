using AdministrationWebApi.Models.Db;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace AdministrationWebApi.Services.SeedDB
{
    public class SeedDB
    {
        public async Task SeedAsync(AppDb db)
        {
            var count = db.Roles.Count();
            if (count < 1)
            {
                var roles = new List<Role>
                {
                    new Role { Name = "super_admin" },
                    new Role { Name = "admin" },
                    new Role { Name = "producer" },
                    new Role { Name = "musician" },
                    new Role { Name = "user" }
                };
                var status_app = new List<StatusApplications>
                {
                    new StatusApplications(){Name = "accepted"},
                    new StatusApplications(){Name = "needs_additional_information"},
                    new StatusApplications(){Name = "rejected"},
                    new StatusApplications(){Name = "new"}
                };
                var _userFaker = new Faker<User>()
                     .RuleFor(u => u.Name, f => f.Name.FullName())
                     .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.Name))
                     .RuleFor(u => u.Birthday, f => f.Date.Past(30, DateTime.Now.AddYears(-18)))
                     .RuleFor(u => u.Role, f => f.PickRandom(roles))
                     .RuleFor(u => u.Password, f => f.Internet.Password(8))
                     .RuleFor(u => u.Avatar, f => f.Internet.Avatar())
                     .RuleFor(u => u.IsEmailVerify, f => f.Random.Bool(0.8f))
                     .RuleFor(u => u.IsBlocked, f => f.Random.Bool(0.1f));
                var users = _userFaker.Generate(50);
                var producers = users.Where(user => user.Role.Name == "producer").Select(user => new Producer() { User = user });
                var musicians = users.Where(user => user.Role.Name == "musician").Select(user => new MemberBand() { User = user });
                var admins = users.Where(user => user.Role.Name == "admin").ToList();

                var bandFaker = new Faker<Band>()
                    .RuleFor(b => b.Name, f => f.Company.CompanyName())
                    .RuleFor(b => b.Producer, f => f.PickRandom(producers))
                    .RuleFor(u => u.Avatar, f => f.Image.PicsumUrl())
                    .RuleFor(b => b.CreatedAt, f => f.Date.Past(5))
                    .RuleFor(b => b.FullInfo, f => f.Lorem.Sentences(2))
                    .RuleFor(b => b.IsBlocked, f => f.Random.Bool(0.2f));
                var bands = bandFaker.Generate(10);

                var blacklist = users
                    .Where(user => user.IsBlocked)
                    .Select(user => new BlacklistUser() { User = user, Admin = admins[new Random().Next(0, admins.Count)] });
                var newsFaker = new Faker<News>()
                    .RuleFor(n => n.Name, f => f.Lorem.Word())
                    .RuleFor(n => n.Author, f => f.PickRandom(bands))
                    .RuleFor(n => n.Text, f => f.Lorem.Paragraphs(3));

                var news = newsFaker.Generate(20);


                db.AddRange(roles);
                db.AddRange(status_app);
                db.AddRange(users);
                db.AddRange(musicians);
                db.AddRange(producers);
                db.AddRange(bands);
                db.AddRange(blacklist);
                db.AddRange(news);
                await db.SaveChangesAsync();
            }

        }
    }
}
