using InventoryApi.Service.SecurityService.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace InventoryApi.Service.SecurityService
{
    public enum KeyType
    {
        refresh,
        access,
        confirmation
    }

    public class JWTUtility: IJWTUtility
    {
        private IOptions<Security> _security;

        private JwtSecurityTokenHandler _securityTokenHandler;

        private RSA accessKey;
        
        private RSA refreshKey;

        private RSA confirmationKey;

        

        private RSA GetKey(KeyType keyType)
        {
            switch (keyType)
            {
                case KeyType.refresh:
                    return refreshKey;
                case KeyType.access:
                    return accessKey;
                case KeyType.confirmation:
                    return confirmationKey;
                default:
                    throw new NotImplementedException();
            };
        }

        private int GetExpiry(KeyType keyType)
        {
            switch (keyType)
            {
                case KeyType.refresh:
                    return _security.Value.RefreshToken.Expiry;
                case KeyType.access:
                    return _security.Value.AccessToken.Expiry;
                case KeyType.confirmation:
                    return _security.Value.ConfirmationToken.Expiry;
                default:
                    throw new NotImplementedException();
            }
            ;
        }

        public JWTUtility(IOptions<Security> security)
        {
            _security = security;
            _securityTokenHandler = new JwtSecurityTokenHandler();
            accessKey = LoadAccessKey();
            refreshKey = LoadRefreshKey();
            confirmationKey = LoadConfirmationKey();
        }

        public async Task<ClaimsIdentity> GetTokenClaims(string? token, KeyType keyType)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("Token is null or empty");
            }

            var result = await _securityTokenHandler.ValidateTokenAsync(token, GetDefaultTokenValidationParams(keyType));
            return result.ClaimsIdentity;
        }
                
        private RSA LoadRefreshKey() => LoadRSAKey(_security.Value.RefreshToken.Key ?? throw new ArgumentNullException("Failed to load RSA Key"));

        private RSA LoadAccessKey() => LoadRSAKey(_security.Value.AccessToken.Key ?? throw new ArgumentNullException("Failed to load RSA Key"));
        private RSA LoadConfirmationKey() => LoadRSAKey(_security.Value.ConfirmationToken.Key ?? throw new ArgumentNullException("Failed to load RSA Key"));

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

        private TokenValidationParameters GetDefaultTokenValidationParams(KeyType keyType)
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = new RsaSecurityKey(GetKey(keyType)),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true
            };
        }


        public bool HasTokenExpired(string? token, KeyType keyType)
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
                    IssuerSigningKey = new RsaSecurityKey(GetKey(keyType)),
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

        public bool IsTokenValid(string? token, KeyType keyType)
        {
            if(string.IsNullOrEmpty(token))
            {
                return false;
            }

            try
            {
                var validations = GetDefaultTokenValidationParams(keyType);
                var claimsPrinciple = _securityTokenHandler.ValidateToken(token, validations, out var validToken);

                return (claimsPrinciple is not null);
            }
            catch {
                return false;
            }
        }

        public string GenerateJWT(IEnumerable<Claim> claims, KeyType keyType)
        {
            var rsa = GetKey(keyType);

            // Create signing credentials with RSA
            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256
            );

            // Configure the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(GetExpiry(keyType)),
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
