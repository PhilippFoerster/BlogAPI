using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BlogAPI.Models.Request;
using BlogAPI.Models.Respond;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Validations;
using Type = BlogAPI.Models.Request.Type;

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
            if (!ModelState.IsValid)
                return BadRequest(new Answer(ModelState.GetErrors(), Type.InvalidModel));
            try
            {
                var user = new User { Email = newUser.Mail, UserName = newUser.Username };
                var res = await userManager.CreateAsync(user, newUser.Password);
                await userManager.AddToRoleAsync(user, "user");
                if (res.Succeeded)
                    return CreatedAtAction("GetUser", new { id = user.Id }, user.GetUserResponse());
                return BadRequest(new Answer(res.Errors.Select(x => x.Description).ToList()));

            }
            catch
            {
                return StatusCode(500, new Answer("Error while creating new user"));
            }
        }

        [HttpGet]
        [Route("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<UsersResponse>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 9)
        {
            try
            {
                var users = await userService.GetUsers(page, pageSize);
                var usersWithRoles = users.Select(async x => new UserWithRolesResponse { User = x, Role = await userService.GetRoles(x.Id) }).Select(x => x.Result).ToList();
                var result = new UsersResponse { TotalCount = await userService.GetUserCount(), Users = usersWithRoles };
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new Answer("Error while getting users"));
            }
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<ActionResult<UserResponse>> GetUser(string id)
        {
            try
            {
                var user = await userService.GetUserResponse(id);
                return user is not null ? Ok(user) : NotFound($"No User with id {id} was found");
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while getting user {id}"));
            }
        }

        [HttpGet]
        [Route("users/{id}/role")]
        public async Task<ActionResult<RolesResponse>> GetUserRoles(string id)
        {
            try
            {
                var role = await userService.GetRoles(id);
                return Ok(new RolesResponse { Role = role });
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while getting role of user {id}"));
            }
        }

        [HttpPost]
        [Route("users/{id}/role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "admin")]
        public async Task<ActionResult<RolesResponse>> UpdateRoles(string id, UpdateRoles role)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Answer(ModelState.GetErrors(), Type.InvalidModel));
            try
            {
                var user = await userService.GetUser(id);
                if (user is null)
                    return NotFound(new Answer($"No user with id {id} was found"));

                var userRole = (await userManager.GetRolesAsync(user)).SingleOrDefault();
                if (userRole == role.Role)
                    return StatusCode(304);
                var res = await userManager.RemoveFromRoleAsync(user, userRole);
                var res2 = await userManager.AddToRoleAsync(user, role.Role);

                if (res.Succeeded && res2.Succeeded)
                    return CreatedAtAction("GetUserRoles", new { id = user.Id }, new RolesResponse { Role = (await userService.GetRoles(user.Id)) });

                var errors = res.Errors.Concat(res2.Errors);
                return BadRequest(new Answer(errors.Select(x => x.Description).ToList()));

            }
            catch(Exception e)
            {
                return StatusCode(500, new Answer($"Error while modifying user {id}"));
            }
        }

        [HttpGet]
        [Route("users/{id}/interests")]
        public async Task<ActionResult<InterestsResponse>> GetUserInterests(string id)
        {
            try
            {
                var interests = await userService.GetInterests(id);
                return Ok(new InterestsResponse { Interests = interests.Select(x => x.Name).ToList() });
            }
            catch
            {
                return StatusCode(500, new Answer($"Error while getting interests of user {id}"));
            }
        }

        [HttpPost]
        [Route("users/interests")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<InterestsResponse>> UpdateInterests(UpdateInterests interests)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Answer(ModelState.GetErrors(), Type.InvalidModel));
            string id = User.GetUserID();
            try
            {
                interests.Interests = interests.Interests.Distinct().ToList();
                var user = await userService.GetUser(id, q => q.Include(x => x.Interests));
                if (user is null)
                    return NotFound(new Answer($"No user with id {id} was found"));

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
            try
            {
                var user = await userService.GetUser(id, x =>
                    x.Include(x => x.LikedComments)
                        .Include(x => x.Comments)
                        .Include(x => x.Articles));
                if (user is null)
                    return NotFound(new Answer($"No user with id {id} was found"));
                await userService.DeleteUserReferences(user);
                var res = await userManager.DeleteAsync(user);
                if (res.Succeeded)
                    return NoContent();
                return BadRequest(new Answer(res.Errors.Select(x => x.Description).ToList()));
            }
            catch (Exception e)
            {
                return StatusCode(500, new Answer("Error while deleting user"));
            }
        }

        [HttpPost]
        [Route("users/login")]
        public async Task<ActionResult<LoginResponse>> Login(Login login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Answer(ModelState.GetErrors(), Type.InvalidModel));
            try
            {
                var user = await userService.GetUserByNameOrMail(login.User);
                if (user is not null && await userManager.CheckPasswordAsync(user, login.Password))
                {
                    var (jwt, refreshToken) = await userService.GetJwtAndRefreshToken(user);
                    return Ok(new LoginResponse { Jwt = jwt, RefreshToken = refreshToken.Token });
                }
                return Unauthorized(new Answer("Username or password is wrong!"));
            }
            catch
            {
                return StatusCode(500, new Answer("Error while login"));
            }
        }

        [HttpPost]
        [Route("users/refreshToken")]
        public async Task<ActionResult<LoginResponse>> RefreshToken(Refresh refresh)
        {
            try
            {
                var userId = await userService.ValidateRefreshToken(refresh);
                if (userId != "")
                {
                    var user = await userService.GetUser(userId);
                    var (jwt, refreshToken) = await userService.GetJwtAndRefreshToken(user);
                    return Ok(new LoginResponse { Jwt = jwt, RefreshToken = refreshToken.Token });
                }
                return Unauthorized(new Answer("Something is wrong with the tokens"));
            }
            catch
            {
                return StatusCode(500, new Answer("Error refreshing token"));
            }
        }

        [HttpGet]
        [Route("users/logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await userService.DeleteRefreshTokensFromUser(User.GetUserID());
                return NoContent();
            }
            catch
            {
                return StatusCode(500, new Answer("Error while logging out"));
            }
        }

    }
}
