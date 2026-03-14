namespace Sysacad.Server.Middleware
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtRefreshMiddleware> _logger;

        public JwtRefreshMiddleware(RequestDelegate next, ILogger<JwtRefreshMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, Services.IJwtService jwtService)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                // Verificar si el token necesita ser refrescado
                if (jwtService.ShouldRefreshToken(token))
                {
                    var newToken = jwtService.RefreshToken(token);

                    if (!string.IsNullOrEmpty(newToken))
                    {
                        _logger.LogInformation("Token refrescado automáticamente. Nuevo token enviado en header jwt-session");
                        token = newToken;
                    }
                }
                context.Response.Headers["jwt-session"] = token;
            }

            await _next(context);
        }
    }

    public static class JwtRefreshMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtRefresh(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtRefreshMiddleware>();
        }
    }
}
