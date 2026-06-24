using System.ComponentModel.DataAnnotations;

namespace Hardware_Service_Cetner.Models;

public class LoginModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;
}