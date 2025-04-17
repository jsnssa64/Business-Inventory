using System.Security.Claims;

namespace InventoryApi.Service.SecurityService
{
    public interface IJWTUtility
    {
        string GenerateJWT(IEnumerable<Claim> claims, bool isRefresh = false);
        Task<ClaimsIdentity> GetTokenClaims(string? token, bool isRefresh = false);
        bool HasTokenExpired(string? token, bool isRefresh = false);
        bool IsTokenValid(string? token, bool isRefresh = false);
    }
}