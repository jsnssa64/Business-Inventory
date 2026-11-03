using System.Security.Claims;
using Domain.ValueObjects.User;

namespace Shared.Utilities.User
{
    public interface IUserUtility
    {
        string GetClaimForUser(IEnumerable<Claim> claims, UserClaim userClaim);
        Claim GetUserClaim(Domain.Entities.User.User user, UserClaim userClaim);
        UserClaims MapClaimsToUser(IEnumerable<Claim> claims);
        IEnumerable<Claim> MapUserToClaims(Domain.ValueObjects.User.UserDetails user);
    }
}
