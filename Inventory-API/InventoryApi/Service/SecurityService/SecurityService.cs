using System.Security.Claims;
using Domain.User;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Service.SecurityService
{
    public class SecurityService: ISecurityService
    {
        private IJWTUtility _JWTUtility;

        public SecurityService(IJWTUtility JWTUtility) 
        {
            _JWTUtility = JWTUtility;
        }

        public void SetCookieForLogin(HttpResponse httpResponse, string accessToken, string refreshToken, DateTimeOffset? expiry)
        {
            var authObj = "{" + 
                $"'AccessToken': '{ accessToken }'," +
                $"'RefreshToken': '{ refreshToken }'" +
            "}";


            httpResponse.Cookies.Append("auth_Bearer", authObj, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = expiry
            });
        }

        public (string AccessToken, string RefreshToken) GenerateLoginJWT(User user)
        {
            if (user.Email.IsNullOrEmpty() || 
                !user.UserRole.HasValue || 
                user.UserName.IsNullOrEmpty())
            {
                throw new Exception($"Unable to Generate Login for {user.Id}");
            }

            var accessClaims = new List<Claim>()
            {
                new("EmailAddress", user.Email),
                new("UserName", user.UserName),
                new("Role", user.UserRole.ToString())
            };

            var refreshClaims = new List<Claim>();

            var keys = _JWTUtility.LoadRsaKeys();

            var accessToken = _JWTUtility.GenerateJWT(accessClaims, keys.accessRsa, false);
            var refreshToken = _JWTUtility.GenerateJWT(refreshClaims, keys.refreshRsa, true);

            return (accessToken, refreshToken);
        }
    }
}
