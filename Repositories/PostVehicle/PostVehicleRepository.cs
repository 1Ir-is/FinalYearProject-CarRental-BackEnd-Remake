using CarRental_BE.Interfaces;
using CarRental_BE.Models.PostVehicle;
using CarRental_BE.Services;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.PostVehicle
{
    public class PostVehicleRepository : IPostVehicleRepository
    {
        private readonly AppDbContext _context;
        private readonly IUploadService _uploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostVehicleRepository(AppDbContext context, IUploadService uploadService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _uploadService = uploadService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task AddPostVehicle(PostVehicleVM ev, long userId)
        {
            var postVehicle = new Entities.PostVehicle()
            {
                /*  Image = ev.Image != null ? await _uploadService.SaveFile(ev.Image) : "",*/
                Category = ev.Category,
                UserId = userId,
                Description = ev.Description,
                Address = ev.Address,
                Price = ev.Price,
                Rating = 0,
                VehicleYear = ev.VehicleYear,
                VehicleType = ev.VehicleType,
                VehicleSeat = ev.VehicleSeat,
                VehicleName = ev.VehicleName,
                VehicleFuel = ev.VehicleFuel,
                Title = ev.Title,
                PlaceId = ev.PlaceId,
                Status = false
            };

            await _context.PostVehicles.AddAsync(postVehicle);
            await _context.SaveChangesAsync();
        }


        public async Task DeletePostVehicle(long id)
        {
            var postVehicle = await _context.PostVehicles.FindAsync(id);

            _context.PostVehicles.Remove(postVehicle);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Entities.PostVehicle>> GetPostVehicles()
        {
            var postVehicles = await _context.PostVehicles
                .Include(x => x.User)
                .Include(x => x.UserRewviewCars).ToListAsync();

            return postVehicles;
        }

        public async Task<Entities.PostVehicle> GetPostVehicle(long id)
        {
            var postVehicle = await _context.PostVehicles
                .Where(x => x.Id == id)
                .Include(x => x.User)
                .Include(x => x.UserRentCars)
                .ThenInclude(x => x.User)
                .Include(x => x.UserRewviewCars)
                .ThenInclude(x => x.User)
                .Include(x => x.FollowVehicles)
                .FirstOrDefaultAsync();

            if (postVehicle == null)
                return null;

            return postVehicle;
        }
        public async Task UpdatePostVehicle(PostVehicleVM ev)
        {
            // Find the post vehicle entity by its ID
            var postVehicle = await _context.PostVehicles.FindAsync(ev.Id);

            // If the post vehicle with the given ID is not found, throw an exception
            if (postVehicle == null)
            {
                throw new ArgumentException($"Post vehicle with ID {ev.Id} not found.");
            }

            // Update the properties of the post vehicle entity with the values from the view model
            postVehicle.Category = ev.Category;
            postVehicle.Description = ev.Description;
            postVehicle.Address = ev.Address;
            postVehicle.Price = ev.Price;
            postVehicle.VehicleYear = ev.VehicleYear;
            postVehicle.VehicleType = ev.VehicleType;
            postVehicle.VehicleSeat = ev.VehicleSeat;
            postVehicle.VehicleName = ev.VehicleName;
            postVehicle.VehicleFuel = ev.VehicleFuel;
            postVehicle.Title = ev.Title;
            postVehicle.PlaceId = ev.PlaceId;

            // Ensure that the required properties are not null or empty
            if (string.IsNullOrEmpty(postVehicle.Category) || string.IsNullOrEmpty(postVehicle.Description)
                || string.IsNullOrEmpty(postVehicle.Address) || string.IsNullOrEmpty(postVehicle.VehicleType)
                || string.IsNullOrEmpty(postVehicle.VehicleName) || string.IsNullOrEmpty(postVehicle.VehicleFuel)
                || string.IsNullOrEmpty(postVehicle.Title) || string.IsNullOrEmpty(postVehicle.PlaceId))
            {
                throw new ArgumentException("One or more required properties are null or empty.");
            }

            // Update other properties as needed

            try
            {
                // Update the post vehicle entity in the database
                _context.PostVehicles.Update(postVehicle);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during the update process
                throw new Exception($"Error updating post vehicle: {ex.Message}");
            }
        }



        public async Task Toggle(long id)
        {
            var postVehicle = await _context.PostVehicles.FindAsync(id);

            postVehicle.Status = !postVehicle.Status;

            _context.PostVehicles.Update(postVehicle);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Entities.PostVehicle>> FindPostVehicles(string search)
        {
            if (string.IsNullOrEmpty(search))
            {
                return await _context.PostVehicles
                .Include(x => x.User)
                .Include(x => x.UserRewviewCars)
                .ToListAsync();
            }
            var key = search.ToLower();
            var postVehicles = await _context.PostVehicles
                .Where(x => x.Category.ToLower().Contains(key) || x.Address.ToLower().Contains(key) || x.VehicleName.Contains(key))
                .Include(x => x.User)
                .Include(x => x.UserRewviewCars)
                .ToListAsync();

            return postVehicles;
        }

        public async Task<IEnumerable<Entities.PostVehicle>> GetPostVehiclesByUser(long userId)
        {
            var post = await _context.PostVehicles.Where(x => x.UserId == userId).Include(x => x.UserRentCars).ToListAsync();

            return post;
        }
    }
}
