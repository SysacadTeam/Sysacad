namespace Sysacad.Server.Middleware
{
    public class OperationCanceledMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<OperationCanceledMiddleware> _logger;

        public OperationCanceledMiddleware(RequestDelegate next, ILogger<OperationCanceledMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogInformation(ex, "Request cancelled by client: {Method} {Path}",
                    context.Request.Method, context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status409Conflict;
            }
        }
    }

    public static class OperationCanceledMiddlewareExtensions
    {
        public static IApplicationBuilder UseOperationCanceled(this IApplicationBuilder builder)
            => builder.UseMiddleware<OperationCanceledMiddleware>();
    }
}
