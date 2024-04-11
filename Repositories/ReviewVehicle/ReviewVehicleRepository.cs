using CarRental_BE.Entities;
using CarRental_BE.Models.ReviewVehicle;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;


namespace CarRental_BE.Repositories.ReviewVehicle
{
    public class ReviewVehicleRepository : IReviewVehicleRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReviewVehicleRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddReview(ReviewVehicleVM vm)
        {
            var res = long.TryParse(_httpContextAccessor.HttpContext.Session.GetString("UserId"), out long userId);
            if (!res)
                return;
            var postVehicle = await _context.PostVehicles.FindAsync(vm.PostVehicleId);
            if (postVehicle == null)
                return;
            var review = new UserReviewVehicle
            {
                UserId = userId,
                PostVehicleId = vm.PostVehicleId,
                Rating = vm.Rating,
                Content = vm.Content,
                Status = true,
                TrustPoint = vm.TrustPoint
            };

            await _context.UserReviewVehicles.AddAsync(review);

            await _context.SaveChangesAsync();

            var user = await _context.Users
                .Where(x => x.Id == userId)
                .Include(x => x.UserReviewVehicles)
                .FirstOrDefaultAsync();
            var lst = user.UserReviewVehicles.Where(x => x.Status).ToList();
            if (lst.Count > 0)
            {
                user.TrustPoint = lst.Average(x => x.TrustPoint);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

            var pv = await _context.PostVehicles
                .Include(x => x.UserRewviewCars)
                .Where(x => x.Id == vm.PostVehicleId)
                .FirstOrDefaultAsync();

            lst = pv.UserRewviewCars.Where(x => x.Status).ToList();
            if (lst.Count > 0)
            {
                pv.Rating = lst.Average(x => x.Rating);
                _context.PostVehicles.Update(pv);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserReviewVehicle>> GetAllReviewVehicles()
        {
            var reviews = await _context.UserReviewVehicles
                .Include(x => x.User)
                .Include(x => x.PostVehicle)
                .ThenInclude(x => x.User)
                .ToListAsync();

            return reviews;
        }

        public async Task Toggle(long id)
        {
            var review = await _context.UserReviewVehicles.FindAsync(id);

            review.Status = !review.Status;

            _context.UserReviewVehicles.Update(review);

            await _context.SaveChangesAsync();

            var user = await _context.Users
                .Where(x => x.Id == review.UserId)
                .Include(x => x.UserReviewVehicles)
                .FirstOrDefaultAsync();
            var lst = user.UserReviewVehicles.Where(x => x.Status).ToList();
            if (lst.Count > 0)
            {
                user.TrustPoint = lst.Average(x => x.TrustPoint);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            var pv = await _context.PostVehicles
                .Include(x => x.UserRewviewCars)
                .Where(x => x.Id == review.PostVehicleId)
                .FirstOrDefaultAsync();

            lst = pv.UserRewviewCars.Where(x => x.Status).ToList();
            if (lst.Count > 0)
            {
                pv.Rating = lst.Average(x => x.Rating);
                _context.PostVehicles.Update(pv);
                await _context.SaveChangesAsync();
            }
        }
    }
}
