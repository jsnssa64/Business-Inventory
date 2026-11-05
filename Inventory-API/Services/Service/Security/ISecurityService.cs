using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Services.Service.Security.Models;
using Services.Service.User.Input;

namespace Services.Service.Security
{
    public interface ISecurityService
    {
        Task<string> HashPassword(string password, SecurityLevel securityLevel = SecurityLevel.Default);
        Task<string> GenerateUserJWT(UserIdInput userIdInput, KeyType keyType);
        Task<(string AccessToken, string RefreshToken)> GenerateLoginJWT(IEnumerable<Claim> accessClaims, IEnumerable<Claim> refreshClaims);
        Task SetCookieForLogin(HttpResponse httpResponse, string accessToken, string refreshToken, DateTimeOffset cookieExpiry);
        Task SetCookieForLogout(HttpResponse httpResponse);
        Task<bool> VerifyPassword(string password, string hashPassword);
        string GetHashFromPayload(string payload, string secret);
        string GenerateSecureSecret(int byteLength = 32);
    }
}
