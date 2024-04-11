using CarRental_BE.Models.RentVehicle;

namespace CarRental_BE.Repositories.RentVehicle
{
    public interface IRentVehicleRepository
    {
        Task RentVehicle(RentVehicleVM vm);
    }
}
