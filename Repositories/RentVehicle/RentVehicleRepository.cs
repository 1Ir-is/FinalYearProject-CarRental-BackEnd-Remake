using CarRental_BE.Entities;
using CarRental_BE.Models.RentVehicle;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.RentVehicle
{
    public class RentVehicleRepository : IRentVehicleRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public RentVehicleRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RentVehicle(RentVehicleVM vm, long userId)
        {
            var rentVehicle = new Entities.UserRentVehicle()
            {
                UserId = userId,
                PostVehicleId = vm.PostVehicleId,
                Name = vm.Name,
                Phone = vm.Phone,
                Email = vm.Email,
                Note = vm.Note,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                CreatedAt = DateTime.Now,
                TotalPrice = vm.TotalPrice.Value
            };

            if (vm.TotalPrice.HasValue)
            {
                rentVehicle.TotalPrice = vm.TotalPrice.Value;
            }
            else
            {
                rentVehicle.TotalPrice = 0; 
            }

            await _context.UserRentVehicles.AddAsync(rentVehicle);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<UserRentVehicle>> GetAllRentersByUserId(long userId)
        {
            try
            {
                var renters = await _context.UserRentVehicles
                    .Where(rv => rv.UserId == userId)
                    .ToListAsync();

                return renters;
            }
            catch (Exception ex)
            {             
                throw ex;
            }
        }

        /*  public async Task<List<UserRentVehicle>> GetRentalDetailsByVehicleId(long vehicleId)
          {
              return await _context.UserRentVehicles
                  .Where(urv => urv.PostVehicleId == vehicleId)
                  .ToListAsync();
          }*/

        public async Task<List<UserRentVehicleDTO>> GetRentalDetailsByVehicleId(long vehicleId)
        {
            try
            {
                var rentalDetails = await _context.UserRentVehicles
                    .Include(urv => urv.PostVehicle) 
                    .Where(urv => urv.PostVehicleId == vehicleId)
                    .Select(urv => new UserRentVehicleDTO
                    {
                        userId = urv.UserId ?? 0, 
                        Name = urv.Name,
                        Phone = urv.Phone,
                        Email = urv.Email,
                        Note = urv.Note,
                        StartDate = urv.StartDate,
                        EndDate = urv.EndDate,
                        TotalPrice = urv.TotalPrice,
                        VehicleName = urv.PostVehicle.VehicleName
                    })
                    .ToListAsync();

                return rentalDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<RentedVehicleDTO>> GetAllVehiclesRentedByUserId(long userId)
        {
            try
            {
                var rentedVehicles = await _context.UserRentVehicles
                    .Include(urv => urv.PostVehicle)
                    .Include(urv => urv.User)
                    .Where(urv => urv.UserId == userId)
                    .Select(urv => new RentedVehicleDTO
                    {
                        VehicleName = urv.PostVehicle.VehicleName,
                        VehicleFuel = urv.PostVehicle.VehicleFuel,
                        VehicleType = urv.PostVehicle.VehicleType,
                        VehicleYear = urv.PostVehicle.VehicleYear ?? 0,
                        VehicleSeat = urv.PostVehicle.VehicleSeat ?? 0,
                        Price = urv.PostVehicle.Price ?? 0,
                        StartDate = urv.StartDate,
                        EndDate = urv.EndDate,
                        UserName = urv.Name,
                        Phone = urv.Phone, // Use phone from rental information
                        Email = urv.Email, // Use email from rental information
                        Name = urv.User.Name, // Assuming you want to keep the user's name
                        Note = urv.Note,
                        CreatedAt = urv.CreatedAt,
                        TotalPrice = (decimal)(urv.EndDate - urv.StartDate).TotalDays * (urv.PostVehicle.Price ?? 0)
                    })
                    .ToListAsync();

                return rentedVehicles;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}

