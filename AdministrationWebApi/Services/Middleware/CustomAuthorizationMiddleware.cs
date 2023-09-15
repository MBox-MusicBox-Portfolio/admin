using System.IdentityModel.Tokens.Jwt;

namespace AdministrationWebApi.Services.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
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

                    if (tokenS.Claims.Any(claim => claim.Type == "Role" && (claim.Value == "admin" || claim.Value == "super_admin")))
                    {                  
                        await _next(context);
                        return;
                    }
                }
                catch (Exception)
                {}
            }

            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized");
            return;
        }
    }
}
