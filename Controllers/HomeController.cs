using CarRental_BE.Repositories.PostVehicle;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IPostVehicleRepository _postVehicleRepository;

        public HomeController (IPostVehicleRepository postVehicleRepository)
        {
            _postVehicleRepository = postVehicleRepository;
        }

        [HttpGet("get-all-post-vehicles")]
        public async Task<IActionResult> GetAllPostVehicles()
        {
            try
            {
                var postVehicles = await _postVehicleRepository.GetPostVehicles();
                return Ok(postVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving post vehicles: {ex.Message}");
            }
        }

        [HttpGet("get-post-vehicle/{id}")]
        public async Task<IActionResult> GetPostVehicle(long id)
        {
            try
            {
                var postVehicle = await _postVehicleRepository.GetPostVehicle(id);

                if (postVehicle == null)
                    return NotFound("Post vehicle not found");

                return Ok(postVehicle);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving post vehicle: {ex.Message}");
            }
        }

        [HttpGet("find-post-vehicles")]
        public async Task<IActionResult> FindPostVehicles([FromQuery] string search)
        {
            try
            {
                var postVehicles = await _postVehicleRepository.FindPostVehicles(search);
                return Ok(postVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error searching post vehicles: {ex.Message}");
            }
        }
    }
}
