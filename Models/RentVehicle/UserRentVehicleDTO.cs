namespace CarRental_BE.Models.RentVehicle
{
    public class UserRentVehicleDTO
    {
        public long userId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
    }

}
