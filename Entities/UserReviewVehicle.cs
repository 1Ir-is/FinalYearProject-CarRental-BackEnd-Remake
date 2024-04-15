using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental_BE.Entities
{
    public class UserReviewVehicle : BaseEntity
    {
        [ForeignKey("UserId")]
        public long? UserId { get; set; }

        [ForeignKey("PostVehicle")]
        public long? PostVehicleId { get; set; }

        public User User { get; set; }
        public PostVehicle PostVehicle { get; set; }
        public bool Status { get; set; } = true;
        public int Rating { get; set; }
        public double TrustPoint { get; set; }
        public string Content { get; set; }
    }
}