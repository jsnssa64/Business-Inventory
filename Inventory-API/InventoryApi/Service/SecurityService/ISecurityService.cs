using System.Security.Claims;
using InventoryApi.Service.SecurityService.Models;

namespace InventoryApi.Service.SecurityService
{
    public interface ISecurityService
    {
        string EncryptPassword(string password, SecurityLevel securityLevel = SecurityLevel.Default);
        (string AccessToken, string RefreshToken) GenerateLoginJWT(IEnumerable<Claim> accessClaims, IEnumerable<Claim> refreshClaims);
        void SetCookieForLogin(HttpResponse httpResponse, string accessToken, string refreshToken, DateTimeOffset cookieExpiry);
        void SetCookieForLogout(HttpResponse httpResponse);
        bool VerifyPassword(string password, string hashPassword);
    }
}