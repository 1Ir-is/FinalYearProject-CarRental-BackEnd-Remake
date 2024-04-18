namespace CarRental_BE.Models.ReviewVehicle
{
    public class ReviewVehicleVM
    {
        public int Rating { get; set; }
        public string Content { get; set; }
        public long PostVehicleId { get; set; }
        public double TrustPoint { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
