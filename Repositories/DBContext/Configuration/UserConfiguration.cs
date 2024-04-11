using CarRental_BE.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarRental_BE.Repositories.DBContext.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<Entities.User>
    {
        public void Configure(EntityTypeBuilder<Entities.User> builder)
        {
            builder.HasData(new Entities.User()
            {
                Id = 1,
                Address = "Hà Nội",
                Email = "admin@admin.com",
                Name = "Admin",
                Password = "$2a$11$pEnPQk9YRgS.JFC7EmWSQOOh9PLscgs4D/q0iykEB8a2xpp.tbj5K", // 123456
                Phone = "0123456789",
                Avatar = "/user-content/default-user.png",
                Role = ROLE_TYPE.ADMIN,
                Status = true
            });
        }
    }
}
