using System;
using System.Collections.Generic;
using System.Linq;
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

                var log_template = @"
                Client IP: {clientIP}
                Request path: {requestPath}
                Request content type: {requestContentType}
                Request content length: {requestContentLength}
                Start time: {startTime}
                Duration: {duration}";

                _logger.LogInformation(
                    log_template,
                    context.Connection.RemoteIpAddress.ToString(),
                    context.Request.Path,
                    context.Request.ContentType,
                    context.Request.ContentLength,
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
