using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Domain.User.Roles;
using System.Security.Claims;

namespace InventoryApi.Authentication
{
    public class MinimumRoleAttribute : Attribute, IAsyncActionFilter
    {
        private readonly RoleLevel _minimumRole;

        public MinimumRoleAttribute(RoleLevel minimumRole)
        {
            _minimumRole = minimumRole;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;

            if (allowAnonymous)
            {
                await next(); // Skip auth check
                return;
            }

            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleClaim = user.FindFirst(ClaimTypes.Role);

            if (roleClaim == null || !Enum.TryParse<RoleLevel>(roleClaim.Value, out var level) || (int)level < (int)_minimumRole)
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }

    }
}
