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

        public JWTUtility(IOptions<Security> security)
        {
            _security = security;
        }
                
        public RSA LoadRefreshKey() => LoadRSAKey(_security.Value.RefreshToken.Key);

        public RSA LoadAccessKey() => LoadRSAKey(_security.Value.AccessToken.Key);

        public RSA LoadRSAKey(string key)
        {
            if(key.IsNullOrEmpty())
            {
                throw new Exception("Unable To Retrieve Keys");
            }

            var rsa = RSA.Create();
            rsa.ImportFromPem(key.ToCharArray());

            return rsa;
        }

        public TokenValidationParameters GetDefaultTokenValidationParams(bool isRefresh = false)
        {
            var key = isRefresh ? LoadRefreshKey() : LoadAccessKey();

            return new TokenValidationParameters
            {
                IssuerSigningKey = new RsaSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true
            };
        }

        public bool ValidForRefresh(string accessToken, string refreshToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            DateTime accessExpired = DateTime.UtcNow;
            ClaimsPrincipal accessClaims;
            try
            {
                var key = LoadAccessKey();

                var accessValidations = new TokenValidationParameters
                {
                    IssuerSigningKey = new RsaSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true
                    //  Without LifeTime Validation
                };

                accessClaims = handler.ValidateToken(accessToken, accessValidations, out var validAccessToken);
                
                //  LifeTime Validation
                if (now >= validAccessToken.ValidTo)
                {
                    var refreshValidations = GetDefaultTokenValidationParams(true);
                    handler.ValidateToken(refreshToken, refreshValidations, out var validRefreshToken);

                    return true;
                }

                return false;                
            }
            catch (Exception ex) {
                throw new Exception("Token is invalid please re-enter credentials");
            }
        }

        public string GenerateJWT(List<Claim> claims, RSA rsa, bool isRefresh)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Create signing credentials with RSA
            var signingCredentials = new SigningCredentials(
                new RsaSecurityKey(rsa),
                SecurityAlgorithms.RsaSha256
            );

            // Configure the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(isRefresh ? _security.Value.RefreshToken.Expiry : _security.Value.AccessToken.Expiry),
                Issuer = _security.Value.Issuer,
                Audience = _security.Value.Audience,
                SigningCredentials = signingCredentials
            };

            // Create and write the token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string EncryptPassword(string password, SecurityLevel securityLevel) =>
            BCrypt.Net.BCrypt.HashPassword(password, (int)securityLevel, BCrypt.Net.SaltRevision.Revision2B);

        public bool VerifyPassword(string password, string hashPassword) => BCrypt.Net.BCrypt.Verify(password, hashPassword);
    }
}
