namespace CarRental_BE.Entities
{
    public class PostVehicle : BaseEntity
    {
        public string? VehicleName { get; set; }
        public string? VehicleFuel { get; set; }
        public string? VehicleType { get; set; }
        public int? VehicleYear { get; set; }
        public int? VehicleSeat { get; set; }

        public string? Description { get; set; }
        public string? Title { get; set; }
        public string? Image { get; set; }
        public string? Category { get; set; }
        public decimal? Price { get; set; }
        public double? Rating { get; set; }
        public string? Address { get; set; }
        public string? PlaceId { get; set; }
        public bool? Status { get; set; } = false;
        public long? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<FollowVehicle>? FollowVehicles { get; set; }
        public ICollection<UserReviewVehicle>? UserRewviewCars { get; set; }
        public ICollection<UserRentVehicle>? UserRentCars { get; set; }
    }
}