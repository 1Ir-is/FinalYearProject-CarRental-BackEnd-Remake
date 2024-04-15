using CarRental_BE.Entities;

namespace CarRental_BE.Models.RentVehicle
{
    public class RentalDetailsViewModel
    {
        public long RentalId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhoneNumber { get; set; }
        public DateTime RentalStartDate { get; set; }
        public DateTime RentalEndDate { get; set; }
        // You can add more properties as needed
    }

}
