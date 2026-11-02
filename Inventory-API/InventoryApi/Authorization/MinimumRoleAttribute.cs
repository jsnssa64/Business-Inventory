using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Domain.ValueObjects.User.Role;
using Domain.ValueObjects.User.Role.Extensions;

namespace InventoryApi.Authorization
{
    public class MinimumRoleAttribute(UserRoleLevel minimumRole) : Attribute, IAsyncActionFilter
    {
        private readonly UserRoleLevel _minimumRole = minimumRole;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var isAnonymouseEndpoint = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;

            if (isAnonymouseEndpoint)
            {
                await next();
                return;
            }

            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var roleClaim = user.FindFirst(ClaimTypes.Role);
            
            if (!UserRolesExtensions.IsValidRole(roleClaim?.Value, out var level) || 
                !UserRolesExtensions.IsHigherThenMinimumRole(level, _minimumRole))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }

    }
}
