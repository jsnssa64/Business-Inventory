using InventoryApi.Service.SecurityService.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace InventoryApi.Service.SecurityService
{
    public class JWTUtility: IJWTUtility
    {
        private IOptions<Security> _security;

        private JwtSecurityTokenHandler _securityTokenHandler;

        private RSA accessKey;
        
        private RSA refreshKey;

        private RSA GetKey(bool isRefresh = false) => isRefresh ? refreshKey : accessKey;
        private int GetExpiry(bool isRefresh = false) => isRefresh ? _security.Value.RefreshToken.Expiry : _security.Value.AccessToken.Expiry;

        public JWTUtility(IOptions<Security> security)
        {
            _security = security;
            _securityTokenHandler = new JwtSecurityTokenHandler();
            accessKey = LoadAccessKey();
            refreshKey = LoadRefreshKey();
        }

        public async Task<ClaimsIdentity> GetTokenClaims(string? token, bool isRefresh = false)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("Token is null or empty");
            }

            var result = await _securityTokenHandler.ValidateTokenAsync(token, GetDefaultTokenValidationParams(isRefresh));
            return result.ClaimsIdentity;
        }
                
        private RSA LoadRefreshKey() => LoadRSAKey(_security.Value.RefreshToken.Key ?? throw new ArgumentNullException("Failed to load RSA Key"));

        private RSA LoadAccessKey() => LoadRSAKey(_security.Value.AccessToken.Key ?? throw new ArgumentNullException("Failed to load RSA Key"));

        private RSA LoadRSAKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Unable To Retrieve Keys");
            }

            var rsa = RSA.Create();
            var decodedKey = Convert.FromBase64String(key);
            // Convert the key string to a ReadOnlySpan<char> and import it
            var pemString = System.Text.Encoding.UTF8.GetString(decodedKey);
            rsa.ImportFromPem(pemString.AsSpan());

            return rsa;
        }

        private TokenValidationParameters GetDefaultTokenValidationParams(bool isRefresh = false)
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = new RsaSecurityKey(GetKey(isRefresh)),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true
            };
        }


        public bool HasTokenExpired(string? token, bool isRefresh = false)
        {
            if (string.IsNullOrEmpty(token))
            {
                return true;
            }

            try
            {
                var now = DateTime.UtcNow;
                var validation = new TokenValidationParameters
                {
                    IssuerSigningKey = new RsaSecurityKey(GetKey(isRefresh)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true
                    //  Without LifeTime Validation
                };

                _securityTokenHandler.ValidateToken(token, validation, out var validToken);

                //  LifeTime Verification
                return (now >= validToken.ValidTo);
            }
            catch {
                return true;
            }
        }

        public bool IsTokenValid(string? token, bool isRefresh = false)
        {
            if(string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var validations = GetDefaultTokenValidationParams(isRefresh);
                var claimsPrinciple = _securityTokenHandler.ValidateToken(token, validations, out var validToken);

                return (claimsPrinciple is not null);
            }
            catch {
                return false;
            }
        }

        public string GenerateJWT(IEnumerable<Claim> claims, bool isRefresh = false)
        {
            var rsa = GetKey(isRefresh);

            // Create signing credentials with RSA
            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256
            );

            // Configure the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(GetExpiry(isRefresh)),
                Issuer = _security.Value.Issuer,
                Audience = _security.Value.Audience,
                SigningCredentials = signingCredentials
            };

            // Create and write the token
            var token = _securityTokenHandler.CreateToken(tokenDescriptor);
            return _securityTokenHandler.WriteToken(token);
        }
    }
}
