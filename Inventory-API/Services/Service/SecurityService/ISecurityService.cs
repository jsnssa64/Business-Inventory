using System.Security.Claims;
using InventoryApi.Service.SecurityService.Models;
using Services.DataModel.User;

namespace InventoryApi.Service.SecurityService
{
    public interface ISecurityService
    {
        string EncryptPassword(string password, SecurityLevel securityLevel = SecurityLevel.Default);
        Task<string> GenerateUserJWT(UserIdentifierModel userIdentifierModel, KeyType keyType);
        (string AccessToken, string RefreshToken) GenerateLoginJWT(IEnumerable<Claim> accessClaims, IEnumerable<Claim> refreshClaims);
        void SetCookieForLogin(HttpResponse httpResponse, string accessToken, string refreshToken, DateTimeOffset cookieExpiry);
        void SetCookieForLogout(HttpResponse httpResponse);
        bool VerifyPassword(string password, string hashPassword);
        string GetHashFromPayload(string payload, string secret);
        string GenerateSecureSecret(int byteLength = 32);
    }
}