using System.ComponentModel.DataAnnotations;

namespace Hardware_Service_Cetner.Models;

public class AccountModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public string? Email { get; set; }
    
    public string? Phone { get; set; }
    public string? Address { get; set; }
    
    public string Username { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    [DataType(DataType.Date)]
    public DateTime RegistrationDate { get; set; } = DateTime.Today;
    
    public bool IsActive { get; set; } = true;
}