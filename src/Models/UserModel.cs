using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{

    public class UserModel
    {
        public Guid UserId { get; set; }
        [Required(ErrorMessage = "User name is required")]
        [MinLength(2, ErrorMessage = "User name must be at least 2 characters long.")]
        [MaxLength(50, ErrorMessage = "User name must be at most 50 characters long.")]
        public required string Name { get; set; }
        [EmailAddress(ErrorMessage = "User email is not valid email")]
        public required string Email { get; set; }
        
        [MinLength(6, ErrorMessage = "User Password must be at least 6 characters.")]
        [Required(ErrorMessage = "User Password is required")]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$")]
        public required string Password { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsBanned { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderModel>? Orders { get; set; } // by Atheer, 1-M relation
    }
}