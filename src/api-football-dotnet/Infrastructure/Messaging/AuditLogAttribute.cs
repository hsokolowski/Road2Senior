using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Messaging;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuditLogAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var started = DateTime.UtcNow;
        var exec = await next();

        var http = ctx.HttpContext;
        ctx.HttpContext.Request.Query.TryGetValue("id", out var id);

        var ev = new AuditEvent(
            Utc: started,
            Action: $"{http.Request.Method} {http.Request.Path}",
            Controller: ctx.Controller.GetType().Name,
            Method: ctx.ActionDescriptor.DisplayName ?? "",
            HttpMethod: http.Request.Method,
            RouteId: id.ToString(),
            StatusCode: exec?.HttpContext.Response.StatusCode,
            Payload: ctx.ActionArguments // np. { id = "...", body = ... }
        );

        var pub = http.RequestServices.GetRequiredService<RabbitPublisher>();
        await pub.PublishAsync(ev);
    }
}
