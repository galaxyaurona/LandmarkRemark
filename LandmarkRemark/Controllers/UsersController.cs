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

        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetUserByUsername(string username)
        {
            // try to get the user with username (ignore case)
            var user = await _userService.GetUserByUsername(username);
            if (user == default(User))
            {
                // add new user if doesn't exist

                return NotFound(new ErrorResponse()
                {
                    Errors = new List<string>()
                    {
                        $"Does not found user with username: {username}"
                    },
                    Message = "User not found"
                });
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateUser(dynamic user)
        {

            // try to add user
            var result = await _userService.AddUserAsync(user?.Username);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetUserByUsername), result.Data);
            }
            else
            {
                return UnprocessableEntity(new ErrorResponse()
                {
                    Errors = result.Errors,
                    Message = "Error creating users"
                });
            }
        }




    }
}
