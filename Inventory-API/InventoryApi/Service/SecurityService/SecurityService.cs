using System.Security.Claims;
using InventoryApi.Constants;
using InventoryApi.Repository;
using InventoryApi.Repository.Data.Product;
using InventoryApi.Service.SecurityService.Models;
using InventoryApi.Service.UserService.Utility;

namespace InventoryApi.Service.SecurityService
{
    public class SecurityService: ISecurityService
    {
        private IJWTUtility _JWTUtility;
        private IUserRepository _userRepository;
        private IUserUtility _userUtility;

        public SecurityService(IJWTUtility JWTUtility, IUserUtility userUtility, IUserRepository userRepository) 
        {
            _JWTUtility = JWTUtility;
            _userRepository = userRepository;
            _userUtility = userUtility;
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
            var accessToken = _JWTUtility.GenerateJWT(accessClaims, KeyType.access);
            var refreshToken = _JWTUtility.GenerateJWT(refreshClaims, KeyType.refresh);

            return (accessToken, refreshToken);
        }

        public string EncryptPassword(string password, SecurityLevel securityLevel = SecurityLevel.Default) =>
            BCrypt.Net.BCrypt.HashPassword(password, (int)securityLevel, BCrypt.Net.SaltRevision.Revision2B);

        public bool VerifyPassword(string password, string hashPassword) => BCrypt.Net.BCrypt.Verify(password, hashPassword);

        public async Task<string> GenerateUserJWT(UserIdentifierModel userIdentifierModel, KeyType keyType)
        {
            var user = await _userRepository.GetUser(userIdentifierModel);

            var claims = _userUtility.MapUserToClaims(user);
            return _JWTUtility.GenerateJWT(claims, keyType);
        }
    }
}
