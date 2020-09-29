using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Twitchbot.Common.Base.Middleware
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
            context.Request.EnableBuffering();

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            var requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation("Path and parameters : {0}/{1} ", context.Request.Path, context.Request.QueryString);
            _logger.LogInformation("Body : {0}", requestBody);

            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation(response);

            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}