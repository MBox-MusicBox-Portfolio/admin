using System.Text;
using System.Text.Json.Serialization;
using AdministrationWebApi.Models.Db;
using AdministrationWebApi.Repositories.Database;
using AdministrationWebApi.Repositories.Database.Interfaces;
using AdministrationWebApi.Repositories.DataBase;
using AdministrationWebApi.Repositories.DataBase.Interfaces;
using AdministrationWebApi.Services.ActionsMailer;
using AdministrationWebApi.Services.DataBase;
using AdministrationWebApi.Services.DataBase.Interfaces;
using AdministrationWebApi.Services.ForAdmin;
using AdministrationWebApi.Services.Middleware;
using AdministrationWebApi.Services.RabbitMQ;
using AdministrationWebApi.Services.ResponseHelper;
using AdministrationWebApi.Services.ResponseHelper.Interfaces;
using AdministrationWebApi.Services.SeedDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MusicBox API", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDb>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.AddAuthentication()
.AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = true,
            ValidIssuer = "config.issuer",
            ValidateAudience = true,
            ValidAudience = "config.audience",
            ValidateLifetime = true,
        };
    });
builder.Services.AddSingleton<RabbitMqService>(sp =>
{
    var connectionFactory = new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMQ:HOST_NAME"],

        UserName = builder.Configuration["RabbitMQ:USER_NAME"],
        Password = builder.Configuration["RabbitMQ:PASSWORD"],
        Ssl = { Enabled = false }
    };
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connection = connectionFactory.CreateConnection();
    return new RabbitMqService(connection, configuration);
});


// services
builder.Services.AddScoped<IActionEventRoute, ActionEventRoute>();
builder.Services.AddScoped<IAdminUserRoleService, AdminUserRoleService>();

builder.Services.AddScoped<IAppllicationsService, ApplicationService>();
builder.Services.AddScoped<IBlacklistService, BlacklistService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IBandService, BandService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<IUserService, UserService>();


//TO DO
builder.Services.AddScoped<IByOneMethod<Album>, AlbumService>();
builder.Services.AddScoped<IByOneMethod<MemberBand>, MemberBandService>();
builder.Services.AddScoped<IByOneMethod<Producer>, ProducerService>();
builder.Services.AddScoped<IBaseService<StatusApplications>, BaseService<StatusApplications>>();
builder.Services.AddScoped<IBaseService<Role>, BaseService<Role>>();
builder.Services.AddScoped<IBaseService<Genre>, BaseService<Genre>>();



// repositories
builder.Services.AddScoped<IEntityRepository<Applications>, ApplicationReposytory>();
builder.Services.AddScoped<IEntityRepository<Album>, AlbumRepository>();
builder.Services.AddScoped<IEntityRepository<BlacklistUser>, BlacklistRepository>();
builder.Services.AddScoped<IEntityRepository<Band>, BandRepository>();
builder.Services.AddScoped<IEntityRepository<MemberBand>, MemberBandRepository>();
builder.Services.AddScoped<IEntityRepository<News>, NewsRepository>();
builder.Services.AddScoped<IEntityRepository<Playlist>, PlaylistRepository>();
builder.Services.AddScoped<IEntityRepository<Producer>, ProducerRepository>();
builder.Services.AddScoped<IEntityRepository<Role>, RoleRepository>();
builder.Services.AddScoped<IEntityRepository<Song>, SongRepository>();
builder.Services.AddScoped<IEntityRepository<StatusApplications>, StatusApplicationsRepository>();
builder.Services.AddScoped<IEntityRepository<User>, UserRepository>();
builder.Services.AddScoped<IEntityRepository<Genre>, GenreRepository>();


builder.Services.AddScoped<IResponseHelper, ResponseHelper>();

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

app.UseSwagger(c =>
{
    c.RouteTemplate = "api/admin/swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/api/admin/swagger/v1/swagger.json", "MusicBox API");
    c.RoutePrefix = "api/admin/swagger";
});

app.UseForwardedHeaders();
app.UseCors(builder => builder
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());

Seed(app);
app.UseMiddleware<CustomAuthorizationMiddleware>();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


async void Seed(IHost host)
{
    using var scope = host.Services.CreateScope();
    var services = scope.ServiceProvider;
    var _context = services.GetRequiredService<AppDb>();
    if (_context != null)
    {
        SeedDB seed = new SeedDB();
        await seed.SeedAsync(_context);
    }
}