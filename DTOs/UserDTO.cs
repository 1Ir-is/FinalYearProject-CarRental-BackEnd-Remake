namespace CarRental_BE.Models.User
{
    public class UserDTO
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; } 
        public double TrustPoint { get; set; } 
        public string Address { get; set; }
    }
}
