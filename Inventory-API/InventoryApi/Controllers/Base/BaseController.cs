using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers.Base
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        public const string V = "[controller]";

        protected string GetUsernameFromClaim()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userName))
                throw new ArgumentNullException(userName, "Username is missing");

            return userName;
        }


        protected string GetUserIdFromClaim()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(userId, "User ID is missing");

            return userId;
        }
    }
}
