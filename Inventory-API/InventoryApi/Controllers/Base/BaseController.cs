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
    }
}
