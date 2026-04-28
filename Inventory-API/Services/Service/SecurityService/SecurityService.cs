using System.Security.Claims;
using System.Security.Cryptography;
using Shared.Constants;
using Microsoft.AspNetCore.Http;
using Services.DataModel.User;
using Services.Repository.UserRepo;
using Shared.Utilities.User;
using Services.Service.SecurityService.Models;

namespace Services.Service.SecurityService
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
            httpResponse.Cookies.Append(JWTCookie.AccessCookie, accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = cookieExpiry
            });

            httpResponse.Cookies.Append(JWTCookie.RefreshCookie, refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = cookieExpiry
            });
        }

        public void SetCookieForLogout(HttpResponse httpResponse)
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

        public string EncryptPassword(string password, SecurityLevel securityLevel = SecurityLevel.Default) =>
            BCrypt.Net.BCrypt.HashPassword(password, (int)securityLevel);

        public bool VerifyPassword(string password, string hashPassword) => BCrypt.Net.BCrypt.Verify(password, hashPassword);

        public string GenerateSecureSecret(int byteLength = 32)
        {
            var bytes = new byte[byteLength];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToHexString(bytes).ToLower(); // hex string
        }

        public string GetHashFromPayload(string payload, string secret)
        {
            var secretBytes = System.Text.Encoding.UTF8.GetBytes(secret);

            using (var hashGenerator = new HMACSHA256(secretBytes))
            {
                var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);
                var hashBytes = hashGenerator.ComputeHash(payloadBytes);

                // Return as lowercase hex string  
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        public async Task<string> GenerateUserJWT(UserIdentifierModel userIdentifierModel, KeyType keyType)
        {
            var user = await _userRepository.GetUser(userIdentifierModel);

            var claims = _userUtility.MapUserToClaims(user);
            return _JWTUtility.GenerateJWT(claims, keyType);
        }
    }
}
