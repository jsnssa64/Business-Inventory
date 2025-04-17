using System.Security.Claims;
using InventoryApi.Constants;
using InventoryApi.Service.SecurityService.Models;

namespace InventoryApi.Service.SecurityService
{
    public class SecurityService: ISecurityService
    {
        private IJWTUtility _JWTUtility;

        public SecurityService(IJWTUtility JWTUtility) 
        {
            _JWTUtility = JWTUtility;
        }

        public void SetCookieForLogin(HttpResponse httpResponse, string accessToken, string refreshToken, DateTimeOffset cookieExpiry)
        {
            httpResponse.Cookies.Append(Cookie.AccessCookie, accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = cookieExpiry
            });

            httpResponse.Cookies.Append(Cookie.RefreshCookie, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = cookieExpiry
            });
        }

        public void SetCookieForLogout(HttpResponse httpResponse)
        {
            httpResponse.Cookies.Delete(Cookie.AccessCookie);
            httpResponse.Cookies.Delete(Cookie.RefreshCookie);
        }

        public (string AccessToken, string RefreshToken) GenerateLoginJWT(IEnumerable<Claim> accessClaims, IEnumerable<Claim> refreshClaims)
        {
            var accessToken = _JWTUtility.GenerateJWT(accessClaims, false);
            var refreshToken = _JWTUtility.GenerateJWT(refreshClaims, true);

            return (accessToken, refreshToken);
        }

        public string EncryptPassword(string password, SecurityLevel securityLevel) =>
            BCrypt.Net.BCrypt.HashPassword(password, (int)securityLevel, BCrypt.Net.SaltRevision.Revision2B);

        public bool VerifyPassword(string password, string hashPassword) => BCrypt.Net.BCrypt.Verify(password, hashPassword);
    }
}
