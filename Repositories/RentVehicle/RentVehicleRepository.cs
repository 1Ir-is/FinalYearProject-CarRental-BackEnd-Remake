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

        public async Task RentVehicle(RentVehicleVM vm)
        {
            var res = long.TryParse(_httpContextAccessor.HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return;

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
                TotalPrice = vm.TotalPrice,
                CreatedAt = DateTime.Now
            };
            await _context.UserRentVehicles.AddAsync(rentVehicle);

            await _context.SaveChangesAsync();
        }
    }
}
