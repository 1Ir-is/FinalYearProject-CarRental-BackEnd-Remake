namespace CarRental_BE.Models.PostVehicle
{
    public class PostVehicleVM
    {
        public string VehicleName { get; set; }
        public string VehicleFuel { get; set; }
        public string VehicleType { get; set; }
        public int VehicleYear { get; set; }
        public int VehicleSeat { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
      /*  public IFormFile Image { get; set; }*/
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string PlaceId { get; set; }
    }
}
