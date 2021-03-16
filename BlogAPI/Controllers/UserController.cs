using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BlogAPI.Attributes;

namespace BlogAPI.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService userService;
        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        [Route("user/new")]
        public async Task<ActionResult<User>> PostUser(NewUser newUser)
        {
            try
            {
                if (!await userService.IsUserUnique(newUser))
                    return Conflict("Username or e-mail is already taken");
                var user = await userService.CreateUser(newUser);
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            catch
            {
                return StatusCode(500, "Error while creating new user");
            }
        }

        [HttpGet]
        [Route("user/{id}")]
        public async Task<ActionResult<User>> GetUser(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var user = await userService.GetUserById((int)id);
                if (user is not null)
                    return user;
                return NotFound($"No User with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting user {id}");
            }
        }

        [HttpPost]
        [Route("user/modify")]
        [Auth(Role.Admin)]
        public async Task<ActionResult<User>> ModifyUser(ModifyUser modifyUser)
        {
            try
            {
                var user = await userService.GetUserById(modifyUser.Id);
                if (user is null)
                    return NotFound($"No User with id {modifyUser.Id} was found");
                user.Role = modifyUser.Role;
                await userService.UpdateUser(user);
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            catch
            {
                return StatusCode(500, $"Error while modifying user {modifyUser.Id}");
            }
        }

    }
}
