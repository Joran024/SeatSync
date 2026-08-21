using SeatSync.Application.Exceptions;
namespace SeatSync.Api.Middleware;
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            var (status, title, code) = ex switch { NotFoundException => (404, ex.Message, "not_found"), ConflictException => (409, ex.Message, "conflict"), ForbiddenException => (403, ex.Message, "forbidden"), UnauthorizedException => (401, ex.Message, "unauthorized"), _ => (500, "An unexpected error occurred.", "server_error") };
            if (status == 500) logger.LogError(ex, "Unhandled request error.");
            context.Response.StatusCode = status; context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new { type = $"https://httpstatuses.com/{status}", title, status, code });
        }
    }
}
