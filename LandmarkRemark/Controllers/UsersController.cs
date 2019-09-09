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
    //protected class ModifyUserRequest : User { }

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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            // try to get the user with username (ignore case)
            var user = await _userService.GetUserByUsername(username);
            if (user == default(User))
            {
                return NoContent();
            }
            else
            {
                return Ok(user);
            }
        }
        private async Task<IActionResult> CreateUser(string username)
        {
            var result = await _userService.AddUserAsync(username);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetUserByUsername), result.Data);
            }
            else
            {
                return UnprocessableEntity(new ErrorResponse()
                {
                    Errors = result.Errors?.ToList(),
                    Message = "Error creating users"
                });
            }
        }


        // Login user, or create user if not found
        [HttpPost("login_or_signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> LoginOrCreate(User request)
        {
            // try to get the user with username (ignore case)
            var user = await _userService.GetUserByUsername(request.Username);
            if (user == default(User))
            {
                return await CreateUser(request.Username);
            }
            else
            {
                return Ok(user);
            }
        }


        // trying to bypass data annotation validation here
        // by using dynamic instead of concrete class
        [HttpPost("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateUser(User user)
        {
            return await CreateUser(user.Username);
        }




    }
}
