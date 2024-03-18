using CarRental_BE.Models.Auth;
using CarRental_BE.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await _userRepository.Login(model);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            // Include user's role in the response data
            var responseData = new
            {
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role // Assuming user.Role is an enum value representing the user's role
            };

            // Return response with user data and role
            return Ok(responseData);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            var success = await _userRepository.Register(model);

            if (!success)
            {
                return Conflict("User with this email already exists");
            }

            return Ok("Registration successful!");
        }
    }
}