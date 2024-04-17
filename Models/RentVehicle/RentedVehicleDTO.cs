namespace CarRental_BE.Models.RentVehicle
{
    public class RentedVehicleDTO
    {
        public string VehicleName { get; set; }
        public string VehicleFuel { get; set; }
        public string VehicleType { get; set; }
        public int VehicleYear { get; set; }
        public int VehicleSeat { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
