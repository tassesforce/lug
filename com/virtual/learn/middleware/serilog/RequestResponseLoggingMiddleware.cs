using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace lug.Middleware.Request
{

    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation(FormatRequest(context.Request));
            
            var bodyStream = context.Response.Body;

            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            await _next(context);

            FormatResponse(context.Response, responseBodyStream);
            await responseBodyStream.CopyToAsync(bodyStream);

        }

        private string FormatRequest(HttpRequest request)
        {
            var injectedRequestStream = new MemoryStream();

        	var requestLog = 
        	$"{request.Scheme} {request.Method} {request.Host}{request.Path} {request.QueryString}, Content-type: {request.ContentType}, JWT: {request.Headers["Authorization"]}";
            using (var bodyReader = new StreamReader(request.Body))
        	{
        		var bodyAsText = bodyReader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(bodyAsText) == false)
                {
        	    	requestLog += $", Body : {bodyAsText}";
        		}

        		var bytesToWrite = Encoding.UTF8.GetBytes(bodyAsText);
        		injectedRequestStream.Write(bytesToWrite, 0, bytesToWrite.Length);
        		injectedRequestStream.Seek(0, SeekOrigin.Begin);
        		request.Body = injectedRequestStream;
        	}
               
            return  requestLog;
        }

        private void FormatResponse(HttpResponse response, MemoryStream responseBodyStream)
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = new StreamReader(responseBodyStream).ReadToEnd();
            _logger.LogInformation($"Response: {responseBody}");
            responseBodyStream.Seek(0, SeekOrigin.Begin);   
        }
    }
}