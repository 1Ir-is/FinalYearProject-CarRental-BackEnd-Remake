
using CarRental_BE.Common.Enums;

namespace CarRental_BE.Entities
{
    public class User : BaseEntity
    {
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Avatar { get; set; }
        public string? Password { get; set; }
        public bool Status { get; set; } = true;
        public double TrustPoint { get; set; } = 0;
        public ROLE_TYPE Role { get; set; }
        public ApprovalApplication ApprovalApplication { get; set; }

        public ICollection<PostVehicle> PostVehicles { get; set; }
        public ICollection<UserRentVehicle> UserRentVehicles { get; set; }
        public ICollection<FollowVehicle> FollowVehicles { get; set; }
        public ICollection<UserReviewVehicle> UserReviewVehicles { get; set; }
    }
}