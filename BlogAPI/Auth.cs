using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlogAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BlogAPI
{
    public class Auth : Attribute, IAsyncActionFilter
    {
        private readonly string[] allowedRoles;
        public Auth(params string[] roles)
        {
            if (roles is null)
                allowedRoles = new[] {"user"};
            else
                allowedRoles = roles;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var roles = context.HttpContext.User.Claims.FirstOrDefault(x =>
                x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            if (roles is null)
                context.Result = new UnauthorizedResult();
            else if (!allowedRoles.Any(x => roles.Value.Contains(x)))
                context.Result = new UnauthorizedResult();
            else
                await next();
        }
    }
}
