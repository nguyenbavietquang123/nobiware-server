using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Web.Utilities;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();

        var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString();
        var method = context.Request.Method;
        var scheme = context.Request.Scheme;
        var host = context.Request.Host;
        var path = context.Request.Path;
        var query = context.Request.QueryString;
        var protocol = context.Request.Protocol; // HTTP/1.1
        var contentType = context.Request.ContentType ?? "-";
        string token = context.Request.Headers["Authorization"].FirstOrDefault();
        var cookies = string.Join("; ", context.Request.Cookies.Select(c => $"{c.Key}={c.Value}"));
        string requestBody = string.Empty;
        context.Request.EnableBuffering();
        if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
        {
            context.Request.Body.Position = 0;
            using (var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            // Reset position so the next middleware/controller can read the body again
            context.Request.Body.Position = 0;
        }

        var fullUrl = $"{scheme}://{host}{path}{query}";
        context.Request.EnableBuffering(); // allows multiple reads
        _logger.LogInformation("Request starting {Protocol} {Method} {Url} - {ContentType} (from {IP}) | Access Token: {token} | Cookies: {Cookies} | Session ID: {Session ID} | Body: {Body}",
            protocol, method, fullUrl, contentType, ip, token, cookies, context.Items["Session_ID"], requestBody);

        await _next(context);

        sw.Stop();
        var statusCode = context.Response.StatusCode;
        contentType = context.Response.ContentType ?? "-";
        _logger.LogInformation("Request finished {Protocol} {Method} {Url} (from {IP}) - {StatusCode} - {ContentType} in {Elapsed}ms | Access Token: {token} | Cookies: {Cookies} | Session ID: {Session ID}",
            protocol, method, fullUrl, ip, statusCode, contentType, sw.ElapsedMilliseconds, token, cookies, context.Items["Session_ID"]);
    }
}
