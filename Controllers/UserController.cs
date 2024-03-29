using CarRental_BE.Entities;
using CarRental_BE.Interfaces;
using CarRental_BE.Models.User;
using CarRental_BE.Repositories.DBContext;
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

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

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

        [HttpGet("check-approval/{userId}")]
        public async Task<IActionResult> CheckApproval(long userId)
        {
            try
            {
                bool isApproving = await _userRepository.IsApproving(userId);
                return Ok(new { IsApproving = isApproving });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
