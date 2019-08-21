using System;
using System.Linq;
using System.Threading.Tasks;
using lug.Context.Correlation;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace lug.Middleware.Correlation
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
    
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }
    
        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers.TryGetValue("Correlation-Id-Header", out var correlationIds);
    
            var correlationId = correlationIds.FirstOrDefault() ?? Guid.NewGuid().ToString();
    
            CorrelationIdContext.SetCorrelationId(correlationId);
    
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                await _next.Invoke(context);
            }
        }
    }
}