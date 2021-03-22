using System.Collections.Generic;
using System.Linq;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace BlogAPI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        private readonly UserManager<IdentityUser> userManager;
        public UserController(UserService userService, UserManager<IdentityUser> userManager)
        {
            this.userService = userService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("user")]
        public async Task<IActionResult> PostUser(NewUser newUser)
        {
            if (newUser.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var user = new User { Email = newUser.Mail, UserName = newUser.Username };
                var res = await userManager.CreateAsync(user, newUser.Password);
                if (res.Succeeded)
                    return CreatedAtAction("GetUser", new { id = user.Id }, new UserResponse { Username = user.UserName, Email = user.Email, Id = user.Id });
                return BadRequest(res.Errors);

            }
            catch
            {
                return StatusCode(500, "Error while creating new user");
            }
        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var user = await userService.GetUserResponse(id);
                return user is not null ? Ok(user) : NotFound($"No User with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting user {id}");
            }
        }

        [HttpPost]
        [Route("users/{id}/roles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateRoles(string id, UpdateRoles roles)
        {
            try
            {
                var user = await userService.GetUser(id);
                if (user is null)
                    return NotFound($"No user with id {id} was found");

                IdentityResult res;
                if (roles.Add)
                    res = await userManager.AddToRolesAsync(user, roles.Roles);
                else
                    res = await userManager.RemoveFromRolesAsync(user, roles.Roles);

                if (res.Succeeded)
                {
                    return CreatedAtAction("GetUser", new { id = user.Id },
                        new UserResponse { Username = user.UserName, Email = user.Email, Id = user.Id, Interests = user.Interests.Select(x => x.Name), Roles = await userManager.GetRolesAsync(user) });
                }
                return BadRequest(res.Errors);
            }
            catch
            {
                return StatusCode(500, $"Error while modifying user {id}");
            }
        }

        [HttpPost]
        [Route("users/{id}/interests")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")] //TODO
        public async Task<IActionResult> UpdateInterests(string id, UpdateInterests interests)
        {
            try
            {
                var user = await userService.GetUser(id);
                if (user is null)
                    return NotFound($"No user with id {id} was found");

                if (interests.Add)
                    user.Interests.AddRange(interests.Interests.Select(x => new Topic { Name = x }));
                else
                {
                    interests.Interests.ForEach(x => user.Interests.Remove(new Topic{Name = x}));
                }
                await userService.UpdateTopics(user.Interests);

                return CreatedAtAction("GetUser", new { id = user.Id },
                    new UserResponse { Username = user.UserName, Email = user.Email, Id = user.Id, Interests = user.Interests.Select(x => x.Name), Roles = await userManager.GetRolesAsync(user) });

            }
            catch
            {
                return StatusCode(500, $"Error while modifying user {id}");
            }
        }

        [HttpDelete]
        [Route("user/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var user = await userManager.FindByIdAsync(id);
                if (user is null)
                    return NotFound($"No user with id {id} was found");
                var res = await userManager.DeleteAsync(user);
                if (res.Succeeded)
                    return NoContent();
                return BadRequest(res.Errors);
            }
            catch
            {
                return StatusCode(500, "Error while deleting user");
            }
        }

        [HttpPost]
        [Route("users/login")]
        public async Task<IActionResult> Login(Login login)
        {
            if (login.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var user = await userManager.FindByNameAsync(login.User) ?? await userManager.FindByEmailAsync(login.User);
                if(user is null)
                    return NotFound("No user was found");
                if (await userManager.CheckPasswordAsync(user, login.Password))
                    return Ok(userService.GenerateJwt(user));
                return Unauthorized("Wrong credentials");

            }
            catch
            {
                return StatusCode(500, "Error while creating new user");
            }
        }

    }
}
