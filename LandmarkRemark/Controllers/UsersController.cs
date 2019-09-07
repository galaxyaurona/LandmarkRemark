using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LandmarkRemark.Models;
using Microsoft.AspNetCore.Http;
using LandmarkRemark.Services;

namespace LandmarkRemark.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await _userService.GetAsync(id);
        }

        [HttpGet("username/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
     
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            // try to get the user with username (ignore case)
            var user = await _userService.GetUserByUsername(username);
            if (user == default(User)) {
                // add new user if doesn't exist
                var newUser = await _userService.AddUserAsync(username);
                return CreatedAtAction(nameof(GetUserByUsername), newUser);
            } else {
                return Ok(user);
            }
        }
        [HttpGet("GenerateException")]
        public async Task GenerateException()
        {
            await _userService.AddUserAsync(null);
        }

    }
}
