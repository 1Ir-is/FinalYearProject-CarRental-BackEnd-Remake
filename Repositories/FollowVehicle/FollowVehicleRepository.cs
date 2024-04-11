using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.FollowVehicle
{
    public class FollowVehicleRepository : IFollowVehicleRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FollowVehicleRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task FollowVehicle(long postVehicleId)
        {
            var res = long.TryParse(_httpContextAccessor.HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return;
            var followVehicle = new Entities.FollowVehicle
            {
                PostVehicleId = postVehicleId,
                UserId = userId
            };

            await _context.FollowVehicles.AddAsync(followVehicle);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Entities.FollowVehicle>> GetAllFollowVehicles()
        {
            var res = long.TryParse(_httpContextAccessor.HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return null;

            var followVehicles = await _context.FollowVehicles
                .Where(x => x.UserId == userId)
                .Include(x => x.PostVehicle)
                .Include(x => x.User)
                .ToListAsync();

            return followVehicles;
        }

        public async Task Toggle(long id)
        {
            var followVehicle = await _context.FollowVehicles.FindAsync(id);

            followVehicle.Status = !followVehicle.Status;
            _context.FollowVehicles.Update(followVehicle);

            await _context.SaveChangesAsync();
        }

        public async Task UnfollowVehicle(long postVehicleId)
        {
            var res = long.TryParse(_httpContextAccessor.HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return;
            var followVehicle = await _context.FollowVehicles
                .FirstOrDefaultAsync(x => x.PostVehicleId == postVehicleId && x.UserId == userId);

            _context.FollowVehicles.Remove(followVehicle);

            await _context.SaveChangesAsync();
        }

    }
}
