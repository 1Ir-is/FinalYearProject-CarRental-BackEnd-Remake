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
                Password = "i2yBMU+FxDo=", // 123456
                Phone = "0123456789",
                Avatar = "/user-content/default-user.png",
                Role = ROLE_TYPE.ADMIN,
                Status = true
            });
        }
    }
}
