using CarRental_BE.Entities;
using CarRental_BE.Models.ReviewVehicle;

namespace CarRental_BE.Repositories.ReviewVehicle
{
    public interface IReviewVehicleRepository
    {
        Task<IEnumerable<Entities.UserReviewVehicle>> GetAllReviewVehicles();

        Task Toggle(long id);

        Task AddReview(ReviewVehicleVM vm, long userId);
        Task<IEnumerable<UserReviewVehicle>> GetReviewVehiclesForPost(long postVehicleId);
    }
}
