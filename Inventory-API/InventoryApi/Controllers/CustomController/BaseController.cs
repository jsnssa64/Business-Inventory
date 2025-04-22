using System.Security.Claims;
using Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers.CustomController
{
    [ApiController]
    public abstract class BaseController : Controller
    {
        protected string GetUsername()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userName))
                throw new Exception("Username is missing");

            return userName;
        }
    }
}
