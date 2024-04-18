
using CarRental_BE.Entities;
using CarRental_BE.Models.RentVehicle;
using CarRental_BE.Models.ReviewVehicle;
using CarRental_BE.Repositories.FollowVehicle;
using CarRental_BE.Repositories.PostVehicle;
using CarRental_BE.Repositories.RentVehicle;
using CarRental_BE.Repositories.ReviewVehicle;
using Microsoft.AspNetCore.Mvc;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IPostVehicleRepository _postVehicleRepository;
        private readonly IRentVehicleRepository _rentVehicleRepository;
        private readonly IFollowVehicleRepository _followVehicleRepository;
        private readonly IReviewVehicleRepository _reviewVehicleRepository;

        public HomeController (IPostVehicleRepository postVehicleRepository, IRentVehicleRepository rentVehicleRepository, IFollowVehicleRepository followVehicleRepository, IReviewVehicleRepository reviewVehicleRepository)
        {
            _postVehicleRepository = postVehicleRepository;
            _rentVehicleRepository = rentVehicleRepository;
            _followVehicleRepository = followVehicleRepository;
            _reviewVehicleRepository = reviewVehicleRepository;
        }


        #region PostVehicle
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
        #endregion PostVehicle


        #region RentVehicle
        [HttpPost("rent-vehicle/{userId}")]
        public async Task<IActionResult> RentVehicle([FromBody] RentVehicleVM vm, long userId)
        {
            try
            {
                await _rentVehicleRepository.RentVehicle(vm, userId);
                return Ok("Vehicle rented successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error renting vehicle: {ex.Message}");
            }
        }

        [HttpGet("get-rental-details/{id}")]
        public async Task<IActionResult> GetRentalDetails(long id)
        {
            try
            {
                var rentalDetails = await _rentVehicleRepository.GetRentalDetailsByVehicleId(id);
                if (rentalDetails == null)
                {
                    return NotFound("Rental details not found");
                }
                return Ok(rentalDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving rental details: {ex.Message}");
            }
        }

        [HttpGet("GetRentalDetailsByVehicleId/{vehicleId}")]
        public async Task<ActionResult<IEnumerable<UserRentVehicleDTO>>> GetRentalDetailsByVehicleId(long vehicleId)
        {
            try
            {
                var rentalDetails = await _rentVehicleRepository.GetRentalDetailsByVehicleId(vehicleId);
                if (rentalDetails == null)
                {
                    return NotFound(); 
                }
                return Ok(rentalDetails); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("get-all-rented-vehicles/{userId}")]
        public async Task<IActionResult> GetAllRentedVehicles(long userId)
        {
            try
            {
                var rentedVehicles = await _rentVehicleRepository.GetAllVehiclesRentedByUserId(userId);
                return Ok(rentedVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving rented vehicles: {ex.Message}");
            }
        }


        #endregion RentVehicle

        #region FollowVehicle

        [HttpGet("get-all-follow-vehicles/{userId}")]
        public async Task<IActionResult> GetAllFollowVehicles(long userId)
        {
            try
            {
                var followVehicles = await _followVehicleRepository.GetAllFollowVehicles(userId);
                return Ok(followVehicles);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error retrieving follow vehicles: {ex.Message}");
            }
        }

        [HttpPost("follow-vehicle")]
        public async Task<IActionResult> FollowVehicle([FromBody] dynamic requestBody)
        {
            try
            {
                long postVehicleId = requestBody.postVehicleId;
                long userId = requestBody.userId;

                if (postVehicleId <= 0 || userId <= 0)
                    return BadRequest("Invalid postVehicleId or userId");

                await _followVehicleRepository.FollowVehicle(postVehicleId, userId);
                return Ok("Vehicle followed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error following vehicle: {ex.Message}");
            }
        }

        [HttpPost("unfollow-vehicle")]
        public async Task<IActionResult> UnfollowVehicle([FromBody] dynamic requestBody)
        {
            try
            {
                long postVehicleId = requestBody.postVehicleId;
                long userId = requestBody.userId;

                if (postVehicleId <= 0 || userId <= 0)
                    return BadRequest("Invalid postVehicleId or userId");

                await _followVehicleRepository.UnfollowVehicle(postVehicleId, userId);
                return Ok("Vehicle unfollowed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error unfollowing vehicle: {ex.Message}");
            }
        }

        #endregion FollowVehicle

        #region ReviewVehicle


        [HttpPost("add-review")]
        public async Task<IActionResult> AddReview([FromBody] ReviewVehicleVM vm, long userId)
        {
            try
            {
                await _reviewVehicleRepository.AddReview(vm, userId);
                return Ok("Review added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding review: {ex.Message}");
            }
        }

        [HttpGet("review-car/{postVehicleId}")]
        public async Task<ActionResult<IEnumerable<UserReviewVehicle>>> GetReviewVehiclesForPost(long postVehicleId)
        {
            var reviewVehicles = await _reviewVehicleRepository.GetReviewVehiclesForPost(postVehicleId);

            if (reviewVehicles == null || !reviewVehicles.Any())
            {
                return NotFound("No review vehicles found for the specified PostVehicleId.");
            }

            return Ok(reviewVehicles);
        }

        #endregion ReviewVehicle



    }
}
