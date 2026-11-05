using Services.Service.Security.Models;
using System.Security.Claims;

namespace Services.Service.Security.Utilities
{
    public interface IJWTUtility
    {
        string GenerateJWT(IEnumerable<Claim> claims, KeyType keyType);
        Task<ClaimsIdentity> GetTokenClaims(string? token, KeyType keyType);
        bool HasTokenExpired(string? token, KeyType keyType);
        bool IsTokenValid(string? token, KeyType keyType);
    }
}