using Services.DataModel.User;
using Services.Service.SecurityService;
using Services.Service.SecurityService.Models;
using Services.Service.UserService;
using Shared.Constants;
using Shared.Utilities.User;
using System.Security.Claims;

namespace InventoryApi.Middleware
{
    public class JwtCookieAuthenticationMiddleware(RequestDelegate next, IJWTUtility JWTUtility, IUserUtility userUtility)
    {
        private readonly RequestDelegate _next = next;
        private readonly IJWTUtility _JWTUtility = JWTUtility;
        private readonly IUserUtility _userUtility = userUtility;

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            string? accessToken = null;
            string? refreshToken = null;
            context.User = new ClaimsPrincipal(new ClaimsIdentity());

            var validAccessToken = context.Request.Cookies.TryGetValue(JWTCookie.AccessCookie, out accessToken);
            var validRefreshToken = context.Request.Cookies.TryGetValue(JWTCookie.RefreshCookie, out refreshToken);

            /*  No Tokens available */
            if (!validAccessToken &&
                !validRefreshToken)
            {
                //  Let attributes deal with the request
                await _next(context);
                return;
            }

            try
            {
                /*  Is Valid Token but has expired */
                if (!_JWTUtility.HasTokenExpired(accessToken, KeyType.access))
                {
                    var claimIdentity = await _JWTUtility.GetTokenClaims(accessToken, KeyType.access);
                    context.User = new ClaimsPrincipal(claimIdentity);

                    await _next(context);
                    return;
                }

                /*  Validate Refresh Token */
                if (_JWTUtility.IsTokenValid(refreshToken, KeyType.refresh))
                {
                    var refreshClaimIdentity = await _JWTUtility.GetTokenClaims(refreshToken, KeyType.refresh);


                    var currentClaims = _userUtility.MapClaimsToUser(refreshClaimIdentity.Claims.ToList());
                    var userIdentifier = new UserIdentifierModel()
                    {
                        Username = currentClaims.Username
                    };

                    var latestUserClaims = await userService.GenerateLogin(context.Response, userIdentifier);

                    context.User = new ClaimsPrincipal(new ClaimsIdentity(latestUserClaims));
                }
            }
            catch (Exception ex)
            {
                userService.LogoutUser(context.Response);
                throw new Exception($"Failed to validate JWT: {ex.Message}");
            }

            await _next(context);
        }
    }
}
