using CarRental_BE.Entities;
using CarRental_BE.Models.RentVehicle;

namespace CarRental_BE.Repositories.RentVehicle
{
    public interface IRentVehicleRepository
    {
        Task RentVehicle(RentVehicleVM vm, long userId);
        Task<IEnumerable<UserRentVehicle>> GetAllRentersByUserId(long userId);
        /*        Task<List<UserRentVehicle>> GetRentalDetailsByVehicleId(long vehicleId);*/
        Task<List<UserRentVehicleDTO>> GetRentalDetailsByVehicleId(long vehicleId);
    }
}
