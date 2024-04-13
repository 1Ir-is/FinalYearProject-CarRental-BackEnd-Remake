using CarRental_BE.Models.PostVehicle;
using CarRental_BE.Repositories.PostVehicle;
using CarRental_BE.Repositories.RentVehicle;
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
        private readonly IRentVehicleRepository _rentVehicleRepository;

        public OwnerController(IPostVehicleRepository postVehicleRepository, IHttpContextAccessor httpContextAccessor, IRentVehicleRepository rentVehicleRepository)
        {
            _postVehicleRepository = postVehicleRepository;
            _httpContextAccessor = httpContextAccessor;
            _rentVehicleRepository = rentVehicleRepository;
        }

       
        [HttpGet("get-post-vehicles-by-user/{userId}")]
        public async Task<IActionResult> GetPostVehiclesByUser(long userId)
        {
            try
            {
                var postVehicles = await _postVehicleRepository.GetPostVehiclesByUser(userId);

                if (postVehicles == null || !postVehicles.Any())
                    return NotFound("No post vehicles found for the specified user");

                return Ok(postVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving post vehicles: {ex.Message}");
            }
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

        [HttpPut("update-post/{postId}")]
        public async Task<IActionResult> UpdatePostVehicle(long postId, [FromBody] UpdateVehicleVM modal)
        {
            try
            {
                await _postVehicleRepository.UpdatePostVehicle(postId, modal);
                return Ok("Post vehicle updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating post vehicle: {ex.Message}");
            }
        }

        [HttpDelete("delete-post/{postId}")]
        public async Task<IActionResult> DeletePostVehicle(long postId)
        {
            try
            {
                await _postVehicleRepository.DeletePostVehicle(postId);
                return Ok("Post vehicle deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting post vehicle: {ex.Message}");
            }
        }

        [HttpGet("get-all-renters/{userId}")]
        public async Task<IActionResult> GetAllRentersByUserId(long userId)
        {
            try
            {
                // Retrieve all renters associated with the specified user
                var renters = await _rentVehicleRepository.GetAllRentersByUserId(userId);

                if (renters == null || !renters.Any())
                    return NotFound("No renters found for the specified user");

                return Ok(renters);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving renters: {ex.Message}");
            }
        }
    }
}
