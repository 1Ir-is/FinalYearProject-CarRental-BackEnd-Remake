using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CarRental_BE.Models.User
{
    public class UserEditVM
    {
        [Required]
        public long userId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }


    }
}
