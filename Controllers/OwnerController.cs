using CarRental_BE.Models.PostVehicle;
using CarRental_BE.Repositories.PostVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IPostVehicleRepository _postVehicleRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OwnerController(IPostVehicleRepository postVehicleRepository, IHttpContextAccessor httpContextAccessor)
        {
            _postVehicleRepository = postVehicleRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("create-post/{userId}")]
        public async Task<IActionResult> AddPostVehicle(PostVehicleVM postVehicleVM, long userId)
        {
            try
            {
               
                await _postVehicleRepository.AddPostVehicle(postVehicleVM, userId);
                return Ok("Post vehicle added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding post vehicle: {ex.Message}");
            }
        }

        [HttpPut("update-post")]
        public async Task<IActionResult> UpdatePostVehicle(PostVehicleVM ev)
        {
            try
            {
                await _postVehicleRepository.UpdatePostVehicle(ev);
                return Ok("Post vehicle updated successfully");
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating post vehicle: {ex.Message}");
            }
        }



    }
}
