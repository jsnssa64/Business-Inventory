using System.Security.Claims;
using Domain.User;

namespace InventoryApi.Service.UserService.Utility
{
    public interface IUserUtility
    {
        string GetClaimForUser(IEnumerable<Claim> claims, UserClaim userClaim);
        Claim GetUserClaim(User user, UserClaim userClaim);
        UserClaims MapClaimsToUser(List<Claim> claims);
        IEnumerable<Claim> MapUserToClaims(User user);
    }
}
