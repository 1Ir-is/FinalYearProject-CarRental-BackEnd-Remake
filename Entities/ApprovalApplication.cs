using CarRental_BE.Common.Enums;

namespace CarRental_BE.Entities
{
    public class ApprovalApplication : BaseEntity
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Identity { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long UserId { get; set; }
        public REQUEST_STATUS RequestStatus { get; set; }
        public APPLICATION_TYPE Type { get; set; }
        public User User { get; set; }
    }
}