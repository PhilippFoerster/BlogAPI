using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BlogAPI.Models.Respond;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Validations;

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
        [Route("users")]
        public async Task<ActionResult<UserResponse>> PostUser(NewUser newUser)
        {
            if (newUser.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var user = new User { Email = newUser.Mail, UserName = newUser.Username };
                var res = await userManager.CreateAsync(user, newUser.Password);
                if (res.Succeeded)
                    return CreatedAtAction("GetUser", new { id = user.Id }, user.GetUserResponse());
                return BadRequest(res.Errors);

            }
            catch
            {
                return StatusCode(500, "Error while creating new user");
            }
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(string id)
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

        [HttpGet]
        [Route("users/{id}/roles")]
        public async Task<ActionResult<RolesResponse>> GetUserRoles(string id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var roles = await userService.GetRoles(id);
                return Ok(new RolesResponse { Roles = roles });
            }
            catch
            {
                return StatusCode(500, $"Error while getting roles of user {id}");
            }
        }

        [HttpPost]
        [Route("users/{id}/roles")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<RolesResponse>> UpdateRoles(string id, UpdateRoles roles)
        {
            try
            {
                var user = await userService.GetUser(id);
                if (user is null)
                    return NotFound($"No user with id {id} was found");

                var userRoles = await userManager.GetRolesAsync(user);
                var intersect = userRoles.Intersect(roles.Roles).ToList();
                var res = await userManager.AddToRolesAsync(user, roles.Roles.Except(intersect));
                var res2 = await userManager.RemoveFromRolesAsync(user, userRoles.Except(intersect));

                if (res.Succeeded)
                    return CreatedAtAction("GetUserRoles", new { id = user.Id }, new RolesResponse { Roles = (await userManager.GetRolesAsync(user)).ToList() });

                var errors = res.Errors.Concat(res2.Errors);
                return BadRequest(errors);
            }
            catch
            {
                return StatusCode(500, $"Error while modifying user {id}");
            }
        }

        [HttpGet]
        [Route("users/{id}/interests")]
        public async Task<ActionResult<InterestsResponse>> GetUserInterests(string id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var interests = await userService.GetInterests(id);
                return Ok(new InterestsResponse { Interests = interests.Select(x => x.Name).ToList() });
            }
            catch
            {
                return StatusCode(500, $"Error while getting interests of user {id}");
            }
        }

        [HttpPost]
        [Route("users/interests")]
        [Authorize]
        public async Task<ActionResult<InterestsResponse>> UpdateInterests(UpdateInterests interests)
        {
            string id = User.GetUserID();
            try
            {
                var user = await userService.GetUserWithInterests(id);
                if (user is null)
                    return NotFound($"No user with id {id} was found");

                var newInterests = interests.Interests.Select(x => new Topic { Name = x }).ToList();

                await userService.UpdateTopics(user, user.Interests, newInterests);

                return CreatedAtAction("GetUserInterests", new { id = user.Id }, new InterestsResponse { Interests = newInterests.Select(x => x.Name).ToList() });

            }
            catch (Exception e)
            {
                return StatusCode(500, $"Error while modifying user {id}");
            }
        }

        [HttpDelete]
        [Route("users/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
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
        public async Task<ActionResult<LoginResponse>> Login(Login login)
        {
            if (login.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                var user = await userManager.FindByNameAsync(login.User) ?? await userManager.FindByEmailAsync(login.User);
                if (user is null)
                    return NotFound("No user was found");
                if (await userManager.CheckPasswordAsync(user, login.Password))
                    return Ok(new LoginResponse{Jwt = await userService.GenerateJwt(user)});
                return Unauthorized("Wrong credentials");

            }
            catch
            {
                return StatusCode(500, "Error while creating new user");
            }
        }
    }
}
