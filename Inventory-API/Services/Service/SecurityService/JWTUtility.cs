using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Service.SecurityService.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Services.Service.SecurityService
{
    public class JWTUtility: IJWTUtility
    {
        private IOptions<Security> _security;

        private JwtSecurityTokenHandler _securityTokenHandler = new JwtSecurityTokenHandler();

        private RSA accessKey;
        
        private RSA refreshKey;

        private RSA confirmationKey;

        private RSA resetPasswordKey;

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
                case KeyType.resetPassword:
                    return resetPasswordKey;
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
                case KeyType.resetPassword:
                    return _security.Value.ResetPasswordToken.Expiry;
                default:
                    throw new NotImplementedException();
            };
        }

        public JWTUtility(IOptions<Security> security)
        {
            _security = security;
            accessKey = LoadAccessKey();
            refreshKey = LoadRefreshKey();
            confirmationKey = LoadConfirmationKey();
            resetPasswordKey = LoadResetPasswordKey();
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
                
        private RSA LoadRefreshKey() => LoadRSAKey(_security.Value.RefreshToken.Key ?? throw FailedToLoadRSAKey);
        private RSA LoadAccessKey() => LoadRSAKey(_security.Value.AccessToken.Key ?? throw FailedToLoadRSAKey);
        private RSA LoadConfirmationKey() => LoadRSAKey(_security.Value.ConfirmationToken.Key ?? throw FailedToLoadRSAKey);
        private RSA LoadResetPasswordKey() => LoadRSAKey(_security.Value.ResetPasswordToken.Key ?? throw FailedToLoadRSAKey);

        private ArgumentNullException FailedToLoadRSAKey => new ArgumentNullException("Failed to load RSA Key");

        private RSA LoadRSAKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Unable To Retrieve Keys");
            }

            var decodedKey = Convert.FromBase64String(key);
            var pemString = System.Text.Encoding.UTF8.GetString(decodedKey);

            var rsa = RSA.Create();
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
                ValidateLifetime = true,
                ValidAudience = _security.Value.Audience,
                ValidIssuer = _security.Value.Issuer
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
                //  Without LifeTime Validation
                var validation = new TokenValidationParameters
                {
                    IssuerSigningKey = new RsaSecurityKey(GetKey(keyType)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidAudience = _security.Value.Audience,
                    ValidIssuer = _security.Value.Issuer
                };

                //  Validate the token and retrieve the valid token
                _securityTokenHandler.ValidateToken(token, validation, out var validToken);

                //  Check if token has expired
                return (DateTime.UtcNow >= validToken.ValidTo);
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
