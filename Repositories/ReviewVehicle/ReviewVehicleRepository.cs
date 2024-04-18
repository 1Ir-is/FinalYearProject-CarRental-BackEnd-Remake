
using CarRental_BE.Entities;
using CarRental_BE.Models.ReviewVehicle;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CarRental_BE.Repositories.ReviewVehicle
{
    public class ReviewVehicleRepository : IReviewVehicleRepository
    {
        private readonly AppDbContext _context;

        public ReviewVehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserReviewVehicle> AddReview(ReviewVehicleVM vm, long userId)
        {
            var postVehicle = await _context.PostVehicles.FindAsync(vm.PostVehicleId);
            if (postVehicle == null)
            {
                throw new ArgumentException("Post vehicle not found");
            }

            var review = new UserReviewVehicle
            {
                UserId = userId,
                PostVehicleId = vm.PostVehicleId,
                Rating = vm.Rating,
                Content = vm.Content,
                Status = true,
                TrustPoint = vm.TrustPoint,
                CreatedDate = DateTime.Now, // Set the CreatedDate property
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

            lst = postVehicle.UserRewviewCars.Where(x => x.Status).ToList();
            if (lst.Count > 0)
            {
                postVehicle.Rating = lst.Average(x => x.Rating);
                _context.PostVehicles.Update(postVehicle);
                await _context.SaveChangesAsync();
            }

            // Return the created review with the createdDate included
            return review;
        }



        public async Task<IEnumerable<UserReviewVehicleDTO>> GetReviewVehiclesForPost(long postVehicleId)
        {
            var reviews = await _context.UserReviewVehicles
                .Where(x => x.PostVehicleId == postVehicleId)
                .Select(x => new UserReviewVehicleDTO
                {
                    Rating = x.Rating,
                    Content = x.Content,
                    TrustPoint = x.TrustPoint,
                    UserName = x.User.Name,
                    UserAvatar = x.User.Avatar,
                    UserId = x.UserId,
                    Date = x.CreatedDate // Assuming there's a property named CreatedDate in UserReviewVehicle representing the creation date
                })
                .ToListAsync();

            return reviews;
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
