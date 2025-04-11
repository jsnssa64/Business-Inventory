using System.Security.Claims;
using System.Security.Cryptography;
using InventoryApi.Service.SecurityService.Models;
using Microsoft.IdentityModel.Tokens;

namespace InventoryApi.Service.SecurityService
{
    public interface IJWTUtility
    {
        string EncryptPassword(string password, SecurityLevel securityLevel);
        string GenerateJWT(List<Claim> claims, RSA rsa, bool isRefresh);
        TokenValidationParameters GetDefaultTokenValidationParams(bool isRefresh = false);
        RSA LoadAccessKey();
        RSA LoadRefreshKey();
        RSA LoadRSAKey(string key);
        bool ValidForRefresh(string accessToken, string refreshToken);
        bool VerifyPassword(string password, string hashPassword);
    }
}