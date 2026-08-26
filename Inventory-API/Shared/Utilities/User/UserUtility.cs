using Domain.ValueObjects.User;
using System.Security.Claims;

namespace Shared.Utilities.User
{
    public class UserUtility: IUserUtility
    {
        public UserClaims MapClaimsToUser(IEnumerable<Claim> claims)
        {
            return new UserClaims()
            {
                Username = GetClaimForUser(claims, UserClaim.Username),
                Email = GetClaimForUser(claims, UserClaim.Email),
                RoleName = GetClaimForUser(claims, UserClaim.Role)
            };
        }

        public IEnumerable<Claim> MapUserToClaims(Domain.Entities.User.User user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var claims = new List<Claim>()
            {
                GetUserClaim(user, UserClaim.Username),
                GetUserClaim(user, UserClaim.Role),
                GetUserClaim(user, UserClaim.Email)
            };
            return claims;
        }

        public Claim GetUserClaim(Domain.Entities.User.User user, UserClaim userClaim)
        {
            switch (userClaim)
            {
                case UserClaim.Email:
                    return new(ClaimTypes.Email, user.Email ?? throw new Exception("Unable to generate Email claim"));      // Ensure non-null value
                case UserClaim.Username:
                    return new(ClaimTypes.Name, user.Username ?? throw new Exception("Unable to generate Username claim"));        // Ensure non-null value
                case UserClaim.Role:
                    return new(ClaimTypes.Role, user.Role.Level.ToString());
                default:
                    throw new NotImplementedException();
            }
        }

        public string GetClaimForUser(IEnumerable<Claim> claims, UserClaim userClaim)
        {
            switch (userClaim)
            {
                case UserClaim.Username:
                    return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? throw new Exception("Unable to generate Username claim");
                case UserClaim.Email:
                    return claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? throw new Exception("Unable to generate Email claim");
                case UserClaim.Role:
                    return claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? throw new Exception("Unable to generate Role claim");
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
