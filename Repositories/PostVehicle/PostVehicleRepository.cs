﻿using CarRental_BE.Models.PostVehicle;
using CarRental_BE.Repositories.DBContext;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.PostVehicle
{
    public class PostVehicleRepository : IPostVehicleRepository
    {
        private readonly AppDbContext _context;

        public PostVehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPostVehicle(PostVehicleVM ev, long userId)
        {
            var postVehicle = new Entities.PostVehicle()
            {
                Image = ev.Image,
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

        public async Task UpdatePostVehicle(long postId, UpdateVehicleVM postVehicleVM)
        {
            try
            {
                var existingPostVehicle = await _context.PostVehicles.FirstOrDefaultAsync(x => x.Id == postId);

                if (existingPostVehicle == null)
                {
                    throw new Exception("Post vehicle not found");
                }

                existingPostVehicle.VehicleName = postVehicleVM.VehicleName ?? existingPostVehicle.VehicleName;
                existingPostVehicle.VehicleFuel = postVehicleVM.VehicleFuel ?? existingPostVehicle.VehicleFuel;
                existingPostVehicle.VehicleType = postVehicleVM.VehicleType ?? existingPostVehicle.VehicleType;
                existingPostVehicle.Description = postVehicleVM.Description ?? existingPostVehicle.Description;
                existingPostVehicle.Title = postVehicleVM.Title ?? existingPostVehicle.Title;
                existingPostVehicle.Category = postVehicleVM.Category ?? existingPostVehicle.Category;
                existingPostVehicle.Address = postVehicleVM.Address ?? existingPostVehicle.Address;
                existingPostVehicle.PlaceId = postVehicleVM.PlaceId ?? existingPostVehicle.PlaceId;

                existingPostVehicle.VehicleYear = postVehicleVM.VehicleYear != 0 ? postVehicleVM.VehicleYear : existingPostVehicle.VehicleYear;
                existingPostVehicle.VehicleSeat = postVehicleVM.VehicleSeat != 0 ? postVehicleVM.VehicleSeat : existingPostVehicle.VehicleSeat;
                existingPostVehicle.Price = postVehicleVM.Price != 0 ? postVehicleVM.Price : existingPostVehicle.Price;


                if (!string.IsNullOrEmpty(postVehicleVM.Image))
                {
                    existingPostVehicle.Image = postVehicleVM.Image;
                }

     
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating post vehicle: {ex.Message}");
            }
        }

        public async Task MarkVehicleAvailable(long postId)
        {
            var postVehicle = await _context.PostVehicles.FindAsync(postId);

            if (postVehicle != null)
            {
                postVehicle.IsRented = false; // Set the status to available
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Post vehicle not found");
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
