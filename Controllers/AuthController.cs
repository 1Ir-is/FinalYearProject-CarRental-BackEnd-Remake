using CarRental_BE.Interfaces;
using CarRental_BE.Models.Auth;
using CarRental_BE.Models.User;
using CarRental_BE.Repositories.User;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailService _mailService;


        public AuthController(IUserRepository userRepository, IMailService mailService)
        {
            _userRepository = userRepository;
            _mailService = mailService;
        }


        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginVM model)
        {
            var user = await _userRepository.Login(model);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            var responseData = new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone,
                Role = user.Role
            };

            // Return response with user data and role
            return Ok(responseData);
        }

        [HttpPost("login-with-google")]
        public async Task<IActionResult> LoginWithGoogle([FromQuery] string googleEmail)
        {
            var user = await _userRepository.LoginWithGoogleEmail(googleEmail);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            var responseData = new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone,
                Role = user.Role
            };

            // Return response with user data and role
            return Ok(responseData);
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginRequest request)
        {
            var user = await _userRepository.LoginWithGoogle(request.Token);

            if (user == null)
            {
                return BadRequest("Failed to login with Google");
            }

            var responseData = new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone,
                Role = user.Role
            };

            return Ok(responseData);
        }
        #endregion Login

        #region Register
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
        #endregion Register


        #region ChangePassword
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            var success = await _userRepository.ChangePasswordUser(model);

            if (!success)
            {
                return BadRequest("Failed to change password. Please check your credentials.");
            }

            return Ok("Password changed successfully!");
        }
        #endregion ChangePassword

        #region Forgot Password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return BadRequest("Email address is required");
                }

                // Generate a unique reset key (you can use Guid or any other method)
                string resetKey = Guid.NewGuid().ToString();

                // Store the reset key in the database along with the user's email and a timestamp
                await _userRepository.StoreResetKey(email, resetKey);

                // Read the content of the ResetPassword.html file
                string filePath = Path.Combine("EmailHtml", "ResetPassword.html");
                string htmlContent = await System.IO.File.ReadAllTextAsync(filePath);

                // Replace placeholders in the HTML content with actual values
                htmlContent = htmlContent.Replace("{resetLink}", $"http://example.com/reset-password?email={email}&resetKey={resetKey}");

                // Send the password reset email
                await _mailService.SendEmailAsync(email, "Password Reset", htmlContent);

                return Ok("Password reset email sent successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }


        #endregion Forgot Password   


        #region Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string resetKey, string newPassword)
        {
            try
            {
                // Verify the reset key from the database
                var result = await _userRepository.VerifyResetKey(email, resetKey);
                if (!result)
                    return NotFound("Invalid or expired reset key");

                // Reset the user's password
                await _userRepository.ResetPassword(email, newPassword);

                return Ok("Password reset successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        #endregion Reset Password

    }
}