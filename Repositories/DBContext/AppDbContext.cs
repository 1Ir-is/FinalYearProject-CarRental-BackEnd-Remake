using CarRental_BE.Repositories.DBContext.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CarRental_BE.Repositories.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserConfiguration).Assembly);
        }

        public DbSet<Entities.User> Users { get; set; }
        public DbSet<Entities.ApprovalApplication> ApprovalApplications { get; set; }
        public DbSet<Entities.PostVehicle> PostVehicles { get; set; }
        public DbSet<Entities.FollowVehicle> FollowVehicles { get; set; }
        public DbSet<Entities.UserRentVehicle> UserRentVehicles { get; set; }
        public DbSet<Entities.UserReviewVehicle> UserReviewVehicles { get; set; }
    }
}