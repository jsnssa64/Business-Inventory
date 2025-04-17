using Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace InventoryApi.Controllers.CustomController
{
    public class BaseController : Controller
    {
        public string GetUsername()
        {
            var userName = User.FindFirst("username")?.Value;

            if (string.IsNullOrEmpty(userName))
                throw new Exception("Username is missing");

            return userName;
        }
    }
}
