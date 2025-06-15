using InventoryApi.Constants;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Service.SecurityService;
using InventoryApi.Service.UserService;
using InventoryApi.Service.UserService.Utility;
using System.Security.Claims;

namespace InventoryApi.Middleware
{
    public class JwtCookieAuthenticationMiddleware(RequestDelegate next, IUserService userService, IJWTUtility JWTUtility, IUserUtility userUtility)
    {
        private readonly RequestDelegate _next = next;
        private readonly IJWTUtility _JWTUtility = JWTUtility;
        private readonly IUserService _userService = userService;
        private readonly IUserUtility _userUtility = userUtility;

        public async Task InvokeAsync(HttpContext context)
        {
            string? accessToken = null;
            string? refreshToken = null;
            context.User = new ClaimsPrincipal(new ClaimsIdentity());

            var validAccessToken = context.Request.Cookies.TryGetValue(Cookie.AccessCookie, out accessToken);
            var validRefreshToken = context.Request.Cookies.TryGetValue(Cookie.RefreshCookie, out refreshToken);

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
                /*  Validate Access Token*/
                if (!_JWTUtility.HasTokenExpired(accessToken, KeyType.access))
                {
                    var claimIdentity = await _JWTUtility.GetTokenClaims(accessToken, KeyType.access);
                    context.User = new ClaimsPrincipal(claimIdentity);

                    await _next(context);
                    return;
                }

                /*  Backup: Validate Refresh Token */
                if (_JWTUtility.IsTokenValid(refreshToken, KeyType.refresh))
                {
                    var refreshClaimIdentity = await _JWTUtility.GetTokenClaims(refreshToken, KeyType.refresh);


                    var currentClaims = _userUtility.MapClaimsToUser(refreshClaimIdentity.Claims.ToList());
                    var userIdentifier = new UserIdentifierModel()
                    {
                        Username = currentClaims.Username
                    };

                    var latestUserClaims = await _userService.GenerateLogin(context.Response, userIdentifier);

                    context.User = new ClaimsPrincipal(new ClaimsIdentity(latestUserClaims));
                }
            }
            catch (Exception ex)
            {
                _userService.LogoutUser(context.Response);
                throw new Exception($"Failed to validate JWT: {ex.Message}");
            }

            await _next(context);
        }
    }
}
