using Domain.User;
using InventoryApi.Constants;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.UserService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace InventoryApi.Middleware
{
    public class JwtCookieAuthenticationMiddleware(RequestDelegate next, IUserService userService, IJWTUtility JWTUtility)
    {
        private readonly RequestDelegate _next = next;
        private readonly IJWTUtility _JWTUtility = JWTUtility;
        private readonly IUserService _userService = userService;

        public async Task InvokeAsync(HttpContext context)
        {
            string? accessToken = null;
            string? refreshToken = null;
            context.User = new ClaimsPrincipal(new ClaimsIdentity());

            /*  No Tokens available */
            if (!context.Request.Cookies.TryGetValue(Cookie.AccessCookie, out accessToken) &&
                !context.Request.Cookies.TryGetValue(Cookie.RefreshCookie, out refreshToken))
            {
                //  Let through
                await _next(context);
                return;
            }

            try
            {
                /*  Valid Access Token*/
                if (!_JWTUtility.HasTokenExpired(accessToken))
                {
                    var claimIdentity = await _JWTUtility.GetTokenClaims(accessToken);
                    context.User = new ClaimsPrincipal(claimIdentity);

                    await _next(context);
                    return;
                }

                /*  Valid Refresh Token */
                if (_JWTUtility.IsTokenValid(refreshToken, true))
                {
                    var refreshClaimIdentity = await _JWTUtility.GetTokenClaims(refreshToken, true);


                    var currentClaims = _userService.MapClaimsToUser(refreshClaimIdentity.Claims.ToList());
                    var userIdentifier = new UserIdentifierModel()
                    {
                        Username = currentClaims.Username
                    };

                    var latestUserClaims = await _userService.RefreshLogin(context.Response, userIdentifier);

                    context.User = new ClaimsPrincipal(new ClaimsIdentity(latestUserClaims));
                }
            }
            catch (Exception ex)
            {
                // TODO: Logout Exception
                throw new Exception($"Failed to validate JWT: {ex.Message}");
            }

            await _next(context);
        }
    }
}
