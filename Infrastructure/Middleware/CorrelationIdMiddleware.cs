namespace WebAPI.Infrastructure.Middleware;

public sealed class CorrelationIdMiddleware {
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger) {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) {
        // 1️. Get or generate correlation ID
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId)) {
            correlationId = Guid.NewGuid().ToString();
        }

        // 2️. Store it for downstream usage
        context.Items[HeaderName] = correlationId;

        // 3️. Add to response headers
        context.Response.OnStarting(() => {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        // 4️. Push into logging scope
        using (_logger.BeginScope(new Dictionary<string, object> {
            ["CorrelationId"] = correlationId
        })) {
            await _next(context);
        }
    }
}
