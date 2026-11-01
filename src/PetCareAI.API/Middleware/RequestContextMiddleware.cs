using System.Security.Claims;
using Serilog.Context;

namespace PetCareAI.API.Middleware;

public class RequestContextMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        var correlationId = ctx.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        ctx.Response.Headers["X-Correlation-Id"] = correlationId;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await next(ctx);
        }
    }
}

public class UserContextMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        var userId = ctx.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonimo";

        using (LogContext.PushProperty("UserId", userId))
        {
            await next(ctx);
        }
    }
}