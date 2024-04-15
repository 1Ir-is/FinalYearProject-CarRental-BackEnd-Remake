using CarRental_BE.Common.Enums;
using CarRental_BE.Entities;
using CarRental_BE.Models.RentVehicle;
using CarRental_BE.Models.User;
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

            // Handle nullable TotalPrice
            if (vm.TotalPrice.HasValue)
            {
                rentVehicle.TotalPrice = vm.TotalPrice.Value; // Explicitly convert nullable decimal? to decimal
            }
            else
            {
                // Handle the case where TotalPrice is null (optional)
                // For example, set a default value
                rentVehicle.TotalPrice = 0; // Set default value to 0 or any other appropriate default value
            }

            await _context.UserRentVehicles.AddAsync(rentVehicle);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<UserRentVehicle>> GetAllRentersByUserId(long userId)
        {
            try
            {
                // Retrieve all renters associated with the specified user
                var renters = await _context.UserRentVehicles
                    .Where(rv => rv.UserId == userId)
                    .ToListAsync();

                return renters;
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                throw ex;
            }
        }

        public async Task<List<UserRentVehicle>> GetRentalDetailsByVehicleId(long vehicleId)
        {
            return await _context.UserRentVehicles
                .Where(urv => urv.PostVehicleId == vehicleId)
                .ToListAsync();
        }

    }
}
