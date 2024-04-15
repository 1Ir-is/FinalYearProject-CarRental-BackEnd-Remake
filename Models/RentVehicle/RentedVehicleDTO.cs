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
        public string UserName { get; set; } // User name
        public string Phone { get; set; } // User phone
        public string Email { get; set; } // User email
        public DateTime CreatedAt { get; set; } // Rental creation date
        public string Name { get; set; } // Name from the form
        public string PhoneNumber { get; set; } // Phone number from the form
    }
}
