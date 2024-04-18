namespace CarRental_BE.Models.ReviewVehicle
{
    public class UserReviewVehicleDTO
    {
        public int Rating { get; set; }
        public string Content { get; set; }
        public double TrustPoint { get; set; }
        public string UserName { get; set; }
        public string UserAvatar { get; set; }
        public long? UserId { get; set; } // Nullable long
        public DateTime Date { get; set; }
    }
}
