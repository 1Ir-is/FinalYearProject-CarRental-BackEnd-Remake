using CarRental_BE.Entities;
using CarRental_BE.Models.ReviewVehicle;

namespace CarRental_BE.Repositories.ReviewVehicle
{
    public interface IReviewVehicleRepository
    {
        Task<IEnumerable<Entities.UserReviewVehicle>> GetAllReviewVehicles();

        Task Toggle(long id);

        Task<UserReviewVehicle> AddReview(ReviewVehicleVM vm, long userId);
        Task<IEnumerable<UserReviewVehicleDTO>> GetReviewVehiclesForPost(long postVehicleId);
    }
}
