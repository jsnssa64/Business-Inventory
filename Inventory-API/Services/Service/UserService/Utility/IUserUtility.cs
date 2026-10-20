using System.Security.Claims;
using Domain.Entities.User;
using Services.Service.UserService.Utility;

namespace Services.Service.UserService.Utility
{
    public interface IUserUtility
    {
        string GetClaimForUser(IEnumerable<Claim> claims, UserClaim userClaim);
        Claim GetUserClaim(User user, UserClaim userClaim);
        UserClaims MapClaimsToUser(IEnumerable<Claim> claims);
        IEnumerable<Claim> MapUserToClaims(User user);
    }
}
