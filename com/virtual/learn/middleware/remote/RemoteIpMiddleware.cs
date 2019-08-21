using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace lug.Middleware.Remote
{
    ///<summary>Ajout de l'adresse IP appelante.null Merci a Shawn Rakowski pour le partage</summary>
    public class RemoteIpMiddleware  
    {
        private readonly RequestDelegate _next;
    
        public RemoteIpMiddleware(RequestDelegate next)
        {
            _next = next;
        }
    
        public async Task Invoke(HttpContext context)
        {
            using (LogContext.PushProperty("Address", context.Connection.RemoteIpAddress))
            {
                await _next.Invoke(context);
            }
        }
    }
}