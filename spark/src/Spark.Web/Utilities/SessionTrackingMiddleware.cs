
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Threading.Tasks;

namespace Spark.Web.Utilities;

public class SessionTrackingMiddleware
{
    private readonly RequestDelegate _next;

    public SessionTrackingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        const string sessionCookieName = "Session_ID";
        // Console.WriteLine("In SessionMiddle");
        if (!context.Request.Cookies.TryGetValue(sessionCookieName, out var sessionId))
        {
            sessionId = Guid.NewGuid().ToString();

            context.Response.Cookies.Append(sessionCookieName, sessionId, new CookieOptions
            {
                HttpOnly = true,
                //Secure = context.Request.IsHttps,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });
        }
        context.Items["Session_ID"] = sessionId;
        await _next(context);
    }
}