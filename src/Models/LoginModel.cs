using System.ComponentModel.DataAnnotations;

namespace api.Model{

public class LoginModel
{
    [Required(ErrorMessage = "User Email is required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "User Password is required")]
    public string Password { get; set; }  = string.Empty;
}}
