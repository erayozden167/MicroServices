using Microsoft.Extensions.Caching.Distributed;

namespace UserAPI.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(RequestDelegate next,
                                    IDistributedCache cache,
                                    IConfiguration configuration,
                                    ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint()?.DisplayName ?? "";
            if (endpoint.Contains("login", StringComparison.OrdinalIgnoreCase))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString();
                var cacheKey = $"ratelimit_{ip}";

                var attempts = await _cache.GetStringAsync(cacheKey);
                var attemptsCount = attempts == null ? 0 : int.Parse(attempts);

                if (attemptsCount >= _configuration.GetValue<int>("RateLimiting:PermitLimit"))
                {
                    _logger.LogWarning($"Rate limit exceeded for IP: {ip}");
                    context.Response.StatusCode = 429;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Too many requests. Please try again later."
                    });
                    return;
                }

                await _cache.SetStringAsync(cacheKey,
                    (attemptsCount + 1).ToString(),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow =
                            TimeSpan.FromSeconds(
                                _configuration.GetValue<int>("RateLimiting:Window"))
                    });
            }

            await _next(context);
        }
    }

}
