using System.Security.Claims;
using System.Security.Cryptography;
using Domain.User;
using InventoryApi.Service.SecurityService.Models;

namespace InventoryApi.Service.SecurityService
{
    public interface ISecurityService
    {
        (string AccessToken, string RefreshToken) GenerateLoginJWT(User user);
        void SetCookieForLogin(HttpResponse httpResponse, string accessToken, string refreshToken, DateTimeOffset? expiry);
    }
}