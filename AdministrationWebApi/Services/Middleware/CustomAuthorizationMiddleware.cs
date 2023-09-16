using System.IdentityModel.Tokens.Jwt;

namespace AdministrationWebApi.Services.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomAuthorizationMiddleware> _logger;
        public CustomAuthorizationMiddleware(RequestDelegate next, ILogger<CustomAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Отримайте токен з запиту
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenS = tokenHandler.ReadJwtToken(token);
                    var userRoleClaim = tokenS.Claims.FirstOrDefault(claim => claim.Type == "Role");
                    if (userRoleClaim != null && (userRoleClaim.Value == "admin" || userRoleClaim.Value == "super_admin"))
                    {
                        context.Items["IsSuperAdmin"] = (userRoleClaim.Value == "super_admin");
                        await _next(context);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred in CustomAuthorizationMiddleware");
                }
            }

            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }
}
