using System.ComponentModel.DataAnnotations;

namespace api.Dtos
{
    public class UpdateUserDto
    {
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long.")]
        [MaxLength(100, ErrorMessage = "Name can not be more 100 characters long.")]
        public string? Name { get; set; }
        
        //[MaxLength(250, ErrorMessage = "Address length must not exceed 250 characters. ")]
        public string? Address { get; set; } 
        public string? Image { get; set; }
        
    }
}