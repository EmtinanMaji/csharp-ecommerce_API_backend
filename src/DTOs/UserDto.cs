using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Dtos
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }

        // public required string Password { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;
        public bool IsBanned { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<OrderModel>? Orders { get; set; } = new List<OrderModel>();
    }
}