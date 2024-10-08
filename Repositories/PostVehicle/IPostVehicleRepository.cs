﻿using CarRental_BE.Entities;
using CarRental_BE.Models.PostVehicle;

namespace CarRental_BE.Repositories.PostVehicle
{
    public interface IPostVehicleRepository
    {
        Task<IEnumerable<Entities.PostVehicle>> GetPostVehicles();
        Task<IEnumerable<Entities.PostVehicle>> GetPostVehiclesByUser(long userId);

        Task<IEnumerable<Entities.PostVehicle>> FindPostVehicles(string search);

        Task<Entities.PostVehicle> GetPostVehicle(long id);
        Task Toggle(long id);
        Task AddPostVehicle(PostVehicleVM ev, long userId);
        Task UpdatePostVehicle(long id, UpdateVehicleVM ev);

        Task DeletePostVehicle(long id);
        Task MarkVehicleAvailable(long postId);
    }
}
