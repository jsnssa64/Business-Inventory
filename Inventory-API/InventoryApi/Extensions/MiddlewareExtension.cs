using InventoryApi.Middleware;

namespace InventoryApi.Extensions
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseJwtCookieAuth(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtCookieAuthenticationMiddleware>();
        }
    }
}
