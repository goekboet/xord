using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Xword.Middleware
{
    public class RequestLogger
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogger> _logger;
        public RequestLogger(RequestDelegate next, ILogger<RequestLogger> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.ToString().Contains("/api/suggest"))
            {
                var start_time = DateTime.UtcNow;

                var watch = Stopwatch.StartNew();
                await _next.Invoke(context);
                watch.Stop();

                var log_template = @"{clientIP} {requestPath} {startTime} {duration}";

                _logger.LogInformation(
                    log_template,
                    context.Connection.RemoteIpAddress.ToString(),
                    context.Request.Path,
                    start_time,
                    watch.ElapsedMilliseconds);
            }
            else
            {
                await _next.Invoke(context);
            }

        }
    }
}
