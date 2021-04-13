using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Identity;

namespace BlogAPI
{
    public class Init
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public Init(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task InitRoles()
        {
            if (!await roleManager.RoleExistsAsync("admin"))
                await roleManager.CreateAsync(new IdentityRole("admin"));
            if (!await roleManager.RoleExistsAsync("user"))
                await roleManager.CreateAsync(new IdentityRole("user"));
            if (!await roleManager.RoleExistsAsync("author"))
                await roleManager.CreateAsync(new IdentityRole("author"));
        }

        public async Task InitAdmin()
        {
            if (await userManager.FindByNameAsync("admin") is null)
            {
                var user = new User{UserName = "Admin",Email = "admin@admin.de"};
                await userManager.CreateAsync(user, "Admin1234!");
                await userManager.AddToRoleAsync(user, "admin");
            }
        }
    }
}
