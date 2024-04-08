using CarRental_BE.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRental_BE.Entities
{
    public class UserRentVehicle : BaseEntity
    {
        [ForeignKey("UserId")]
        public long? UserId { get; set; }

        [ForeignKey("PostVehicle")]
        public long? PostVehicleId { get; set; }

        public User User { get; set; }
        public PostVehicle PostVehicle { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}