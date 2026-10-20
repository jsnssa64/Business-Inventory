using Services.Service.SecurityService.Models;
using System.Security.Claims;

namespace Services.Service.SecurityService
{
    public interface IJWTUtility
    {
        string GenerateJWT(IEnumerable<Claim> claims, KeyType keyType);
        Task<ClaimsIdentity> GetTokenClaims(string? token, KeyType keyType);
        bool HasTokenExpired(string? token, KeyType keyType);
        bool IsTokenValid(string? token, KeyType keyType);
    }
}