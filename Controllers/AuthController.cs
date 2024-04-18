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
        private readonly ITokenService _tokenService;


        public AuthController(IUserRepository userRepository, IMailService mailService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _mailService = mailService;
            _tokenService = tokenService;
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

            // Generate token
            var token = _tokenService.CreateToken(user);

            var responseData = new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone,
                Role = user.Role,
                Token = token // Include token in response
            };

            // Return response with user data and token
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

            // Generate token
            var token = _tokenService.CreateToken(user);

            var responseData = new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone,
                Role = user.Role,
                Token = token // Include token in response
            };

            // Return response with user data and token
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

            // Generate token
            var token = _tokenService.CreateToken(user);

            var responseData = new
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                Phone = user.Phone,
                Role = user.Role,
                Token = token // Include token in response
            };

            // Return response with user data and token
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

            // Optionally, you can generate a token for the newly registered user
            var user = await _userRepository.GetUserByEmail(model.Email);
            var token = _tokenService.CreateToken(user);

            return Ok(new { Message = "Registration successful!", Token = token });
        }

        #endregion Register

        #region ToggleUserStatus
        [HttpPost("toggle/{id}")]
        public async Task<IActionResult> ToggleUserStatus(long id)
        {
            try
            {
                var success = await _userRepository.Toggle(id);

                if (!success)
                {
                    return NotFound("User not found or failed to toggle status");
                }

                return Ok("User status toggled successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        #endregion ToggleUserStatus


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

                // Store the reset key and timestamp in the database
                DateTime timestamp = DateTime.UtcNow; // Get current UTC time
                await _userRepository.StoreResetKey(email, resetKey, timestamp);

                // Read the content of the ResetPassword.html file
                string filePath = Path.Combine("EmailHtml", "ResetPassword.html");
                string htmlContent = await System.IO.File.ReadAllTextAsync(filePath);

                // Replace placeholders in the HTML content with actual values
                htmlContent = htmlContent.Replace("{resetLink}", $"http://localhost:3000/reset-password/{email}/{resetKey}");

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
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            try
            {
                // Check if reset key is valid and not expired
                var resetKeyInfo = await _userRepository.GetResetKeyInfo(model.Email, model.ResetKey);
                if (resetKeyInfo == (null, null) || (DateTime.UtcNow - resetKeyInfo.ResetKeyTimestamp.Value).TotalMinutes > 15)
                {
                    // Reset key is invalid or expired
                    return BadRequest("Reset key is invalid or expired");
                }

                // Reset the user's password
                await _userRepository.ResetPassword(model.Email, model.NewPassword);

                return Ok("Password reset successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        #endregion Reset Password

        [HttpPost("contact")]
        public async Task<IActionResult> SendMessage(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string subject = "New Contact Form Submission";
            string body = $"Name: {model.Name}<br>Email: {model.Email}<br>Message: {model.Message}";

            try
            {
                // Send email using MailService
                await _mailService.SendEmailAsync("arsherwinphonguniverse@gmail.com", subject, body);
                return Ok("Message sent successfully");
            }
            catch (Exception ex)
            {
                // Log error and return error response
                Console.WriteLine($"Error sending email: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

    }
}