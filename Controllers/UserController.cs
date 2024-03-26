using CarRental_BE.Entities;
using CarRental_BE.Interfaces;
using CarRental_BE.Models.User;
using CarRental_BE.Repositories.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IUploadService _uploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(AppDbContext context, IUploadService uploadService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _uploadService = uploadService;
            _httpContextAccessor = httpContextAccessor;
        }



        [HttpPost("edit-info")]
        public async Task<IActionResult> EditUserInfo([FromBody] UserEditVM vm)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == vm.userId);
                if (user == null)
                {
                    return NotFound("User not found");
                }

                user.Name = vm.Name;
                user.Address = vm.Address;
                user.Phone = vm.Phone;

                _context.Users.Update(user);
                var success = await _context.SaveChangesAsync() > 0;

                return Ok("User information updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
