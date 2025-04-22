using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Domain.User.Roles;
using System.Security.Claims;

namespace InventoryApi.Authentication
{
    public class MinimumRoleAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly RoleLevel _minimumRole;

        public MinimumRoleAttribute(RoleLevel minimumRole)
        {
            _minimumRole = minimumRole;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role);

            if (roleClaim == null ||
                !int.TryParse(roleClaim.Value, out var roleLevel) ||
                roleLevel < (int)_minimumRole)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}
