namespace CarRental_BE.Repositories.FollowVehicle
{
    public interface IFollowVehicleRepository
    {
        Task FollowVehicle(long postVehicleId, long userId);

        Task UnfollowVehicle(long postVehicleId, long userId);

        Task<IEnumerable<Entities.FollowVehicle>> GetAllFollowVehicles(long userId);

        Task Toggle(long id);
    }
}
