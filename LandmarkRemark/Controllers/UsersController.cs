using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LandmarkRemark.Models;
using Microsoft.AspNetCore.Http;

namespace LandmarkRemark.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly LandmarkRemarkContext _context;

        public UsersController(LandmarkRemarkContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(e => e.Id == id);
        }

        [HttpGet("username/{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // TODO: 
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            // try to get the user with username (ignore case)
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase) );
            if (user == default(User)) {
                // add new user if doesn't exist
                var newUser = await _context.Users.AddAsync( new User(){
                    Username = username
                });
                return CreatedAtAction(nameof(GetUserByUsername), newUser);
            } else {
                return Ok(user);
            }
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
