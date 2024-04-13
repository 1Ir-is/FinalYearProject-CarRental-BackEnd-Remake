using CarRental_BE.Entities;
using CarRental_BE.Interfaces;
using CarRental_BE.Models.User;
using CarRental_BE.Repositories.DBContext;
using CarRental_BE.Repositories.PostVehicle;
using CarRental_BE.Repositories.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPostVehicleRepository _postVehicleRepository;

        public UserController(IUserRepository userRepository, IPostVehicleRepository postVehicleRepository)
        {
            _userRepository = userRepository;
            _postVehicleRepository = postVehicleRepository;
        }


        #region GetUser
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAll(); // Call the GetAll method from the repository
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            try
            {
                // Call the repository method to fetch the user by ID
                var user = await _userRepository.GetById(id);

                if (user == null)
                    return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("avatar/{userId}")]
        public async Task<IActionResult> GetUserAvatar(long userId)
        {
            try
            {
                // Call the repository method to fetch the avatar URL
                var userAvatar = await _userRepository.GetUserAvatar(userId);

                // Return the avatar URL in the response
                return Ok(new { Avatar = userAvatar });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion GetUser



        #region EditInfo
        
        [HttpPost("edit-info")]
        public async Task<IActionResult> EditUserInfo([FromBody] UserEditVM vm)
        {
            try
            {
                await _userRepository.EditInfoUser(vm);
                return Ok("User information updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("create-approval-application/{userId}")] // Add userId as a parameter in the URL
        public async Task<IActionResult> CreateApprovalApplication([FromBody] ApprovalApplicationVM vm, long userId)
        {
            try
            {
                // Call the repository method to create the approval application
                await _userRepository.CreateApprovalApplication(vm, userId);
                return Ok("Approval application created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion EditInfo


        #region CheckApproval

        [HttpGet("check-approval/{userId}")]
        public async Task<IActionResult> CheckApproval(long userId)
        {
            try
            {
                string isApproving = await _userRepository.GetRequestStatus(userId);
                return Ok(new { IsApproving = isApproving });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion CheckApproval

    }
}
