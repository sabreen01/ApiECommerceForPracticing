namespace MyEcommerce.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Before
        var startTime = DateTime.UtcNow;
        logger.LogInformation(
            "Request: {Method} {Path} started at {Time}",
            context.Request.Method,
            context.Request.Path,
            startTime);

        await next(context);

        // After
        var duration = DateTime.UtcNow - startTime;
        logger.LogInformation(
            "Response: {StatusCode} completed in {Duration}ms",
            context.Response.StatusCode,
            duration.TotalMilliseconds);
    }
}

// Extension method for clean registration
public static class RequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
