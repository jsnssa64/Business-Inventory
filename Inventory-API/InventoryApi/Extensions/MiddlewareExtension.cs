using InventoryApi.Middleware;

namespace InventoryApi.Extensions
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseJwtCookieAuthMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<JwtCookieAuthenticationMiddleware>();
        }

        public static IApplicationBuilder UseGlobalExceptionHandlerMiddleware(this IApplicationBuilder app)
        {
            throw new NotImplementedException("Global exception handler middleware is not implemented yet.");
        }
    }
}
