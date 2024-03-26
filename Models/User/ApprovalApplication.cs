using CarRental_BE.Common.Enums;

namespace CarRental_BE.Models.User
{
    public class ApprovalApplication
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Identity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public APPLICATION_TYPE Type { get; set; }
    }
}
