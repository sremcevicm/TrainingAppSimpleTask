using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainingApp.Server.Data.Contexts;
using TrainingApp.Server.Interfaces;
using TrainingApp.Server.Services;
using TrainingApp.Shared.DTOs;

namespace TrainingApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase // Fix: Inherit from ControllerBase to access BadRequest and other helper methods
    {
        public IUser _service;

        public UserController(IUser service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _service.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("check-or-add")]
        public async Task<IActionResult> CheckOrAddUser([FromBody] UserDetailsDTO user)
        {
            if (user == null || string.IsNullOrEmpty(user.Email))
                return BadRequest("Neispravni podaci");

            var existingUser = await _service.GetUserByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return Ok(existingUser); // Vrati postojeceg korisnika
            }

            var addedUser = await _service.AddUserAsync(user);
            return Ok(addedUser); // Vrati novog korisnika
        }

        [HttpPut("{email}")]
        public async Task<IActionResult> UpdateUser(string email, UserDetailsDTO updatedUser)
        {
            var success = await _service.UpdateUserByEmailAsync(email, updatedUser);
            if (!success)
                return NotFound(); 

            return NoContent();
        }

    }
}
