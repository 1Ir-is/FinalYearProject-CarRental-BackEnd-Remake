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

 /*       public async Task UpdatePostVehicle(PostVehicleVM ev)
        {
            var postVehicle = await _context.PostVehicles.FindAsync(ev.Id);

            postVehicle.Category = ev.Category;
            postVehicle.Description = ev.Description;
            postVehicle.Address = ev.Address;
            postVehicle.Price = ev.Price;
            postVehicle.Rating = 0;
            postVehicle.VehicleYear = ev.VehicleYear;
            postVehicle.VehicleType = ev.VehicleType;
            postVehicle.VehicleSeat = ev.VehicleSeat;
            postVehicle.VehicleName = ev.VehicleName;
            postVehicle.VehicleFuel = ev.VehicleFuel;
            postVehicle.Title = ev.Title;
            postVehicle.PlaceId = ev.PlaceId;

        *//*    var img = postVehicle.Image;
            if (ev.Image != null)
                postVehicle.Image = await _uploadService.SaveFile(ev.Image);*//*

            _context.PostVehicles.Update(postVehicle);
            await _context.SaveChangesAsync();

         *//*   if (ev.Image != null)
                await _uploadService.DeleteFile(img);*//*
        }*/

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
