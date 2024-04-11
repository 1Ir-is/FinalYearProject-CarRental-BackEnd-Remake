namespace CarRental_BE.Repositories.FollowVehicle
{
    public interface IFollowVehicleRepository
    {
        Task FollowVehicle(long postVehicleId);

        Task UnfollowVehicle(long postVehicleId);

        Task<IEnumerable<Entities.FollowVehicle>> GetAllFollowVehicles();

        Task Toggle(long id);
    }
}
