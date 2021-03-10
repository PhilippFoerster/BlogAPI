using BlogAPI.Database;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Controllers
{
    [ApiController]
    [Route("user/new")]
    public class UserController : ControllerBase
    {
        UserService userService;
        public UserController(UserService userService)
        {
            this.userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (await userService.CreateUser(user))
                return StatusCode(201, new Response(false, "created successfully"));
            return StatusCode(400, new Response(true, "error while creating new user"));
        }

    }
}
