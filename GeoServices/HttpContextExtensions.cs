using Microsoft.AspNetCore.Http;

namespace GeoServices
{
    public static class HttpContextExtensions
    {
        public static string? GetRemoteIpAddress(this HttpContext ctx)
        {
            var ipAddress = ctx.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? ctx.Connection.RemoteIpAddress?.ToString();
            return ipAddress;
        }
    }
}
