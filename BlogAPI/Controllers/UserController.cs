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
        [Route("user")]
        public async Task<ActionResult<User>> PostUser(NewUser newUser)
        {
            if (newUser.HasNullProperty())
                return BadRequest("Missing properties!");
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
                var user = await userService.GetUser((int)id);
                return user is not null ? user : NotFound($"No User with id {id} was found");
            }
            catch
            {
                return StatusCode(500, $"Error while getting user {id}");
            }
        }

        [HttpPut]
        [Route("user")]
        [Auth(Role.Admin)]
        public async Task<ActionResult<User>> ModifyUser(UpdateUser updateUser)
        {
            try
            {
                var user = await userService.GetUser(updateUser.Id);
                if (user is null)
                    return NotFound($"No user with id {updateUser.Id} was found");
                user.UpdateFrom(updateUser);
                await userService.UpdateUser(user);
                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            catch
            {
                return StatusCode(500, $"Error while modifying user {updateUser.Id}");
            }
        }

        [HttpDelete]
        [Route("user/{id}")]
        [Auth(Role.Admin)]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (id is null)
                return BadRequest("Missing id");
            try
            {
                var user = await userService.GetUser((int)id);
                if (user is null)
                    return NotFound($"No user with id {id} was found");
                await userService.DeleteUser(user);
                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Error while deleting user");
            }
        }

    }
}
