using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogAPI.Models;
using BlogAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogAPI.Controllers
{

    [ApiController]
    public class IdentityContoller : ControllerBase
    {
        private IdentityService identityService;

        public IdentityContoller(IdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost]
        [Route("user/login")]
        public async Task<IActionResult> Login(NewUser newUser)
        {
            if (newUser.HasNullProperty())
                return BadRequest("Missing properties!");
            try
            {
                return Ok(identityService.GenerateJwt(newUser.Username, newUser.Mail));
            }
            catch
            {
                return StatusCode(500, "Error while creating new user");
            }
        }
    }
}
