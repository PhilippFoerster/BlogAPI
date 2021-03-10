using BlogAPI.Database;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Auth : Attribute, IAsyncActionFilter
    {
        private Role[] roles;
        public Auth(params Role[] roles)
        {
            this.roles = roles;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            UserService userService = (UserService)context.HttpContext.RequestServices.GetService(typeof(UserService));
            if (userService.IsAuthorized(context.HttpContext, roles))
                await next();
            else
                context.Result = new UnauthorizedResult();
        }
    }

    public enum Role { User, Author, Admin }
}
