using System.ComponentModel.DataAnnotations;

namespace CarRental_BE.Entities
{
    public class BaseEntity
    {
        [Key]
        public long Id { get; set; }
    }
}
