using System.Security.Claims;
using System.Security.Cryptography;
using Shared.Constants;
using Microsoft.AspNetCore.Http;
using Shared.Utilities.User;
using Domain.Interface;
using Services.Service.Security.Models;
using Services.Service.Security.Utilities;
using Domain.ValueObjects.User;

namespace Services.Service.Security
{
    public class SecurityService(IJWTUtility JWTUtility, IUserUtility userUtility, IUserRepository userRepository) : ISecurityService
    {
        private readonly IJWTUtility _JWTUtility = JWTUtility;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUserUtility _userUtility = userUtility;

        //  TODO: This is static data setup and probably should not be here as this may change depending on envrinoment
        private static CookieOptions CreateCookieOptions(DateTimeOffset cookieExpiry) => new()
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = cookieExpiry
        };

        public static void SetCookieForLogin(HttpResponse httpResponse, string accessToken, string refreshToken, DateTimeOffset cookieExpiry)
        {
            httpResponse.Cookies.Append(JWTCookie.AccessCookie, accessToken, CreateCookieOptions(cookieExpiry));
            httpResponse.Cookies.Append(JWTCookie.RefreshCookie, refreshToken, CreateCookieOptions(cookieExpiry));
        }


        public static void SetCookieForLogout(HttpResponse httpResponse)
        {
            httpResponse.Cookies.Delete(JWTCookie.AccessCookie);
            httpResponse.Cookies.Delete(JWTCookie.RefreshCookie);
        }

        public (string AccessToken, string RefreshToken) GenerateLoginJWT(IEnumerable<Claim> accessClaims, IEnumerable<Claim> refreshClaims)
        {
            var accessToken = _JWTUtility.GenerateJWT(accessClaims, KeyType.access);
            var refreshToken = _JWTUtility.GenerateJWT(refreshClaims, KeyType.refresh);

            return (accessToken, refreshToken);
        }

        public static string EncryptPassword(string password, SecurityLevel securityLevel = SecurityLevel.Default)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, (int)securityLevel);
        }

        public static bool VerifyPassword(string password, string hashPassword) => BCrypt.Net.BCrypt.Verify(password, hashPassword);

        public static string GenerateSecureSecret(int byteLength = 32)
        {
            var bytes = new byte[byteLength];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToHexString(bytes).ToLower();
        }

        public static string GetHashFromPayload(string payload, string secret)
        {
            var secretBytes = System.Text.Encoding.UTF8.GetBytes(secret);

            using var hashGenerator = new HMACSHA256(secretBytes);
            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
            var hashBytes = hashGenerator.ComputeHash(payloadBytes);

            // Return as lowercase hex string  
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }

        public async Task<string> GenerateUserJWT(UserId userIdentifierModel, KeyType keyType)
        {
            var user = await _userRepository.GetUser(userIdentifierModel)
                ?? throw new KeyNotFoundException($"User not found for username: {userIdentifierModel.Username}");

            var claims = _userUtility.MapUserToClaims(user);
            return _JWTUtility.GenerateJWT(claims, keyType);
        }
    }
}
