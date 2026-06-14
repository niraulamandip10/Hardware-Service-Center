using System.ComponentModel.DataAnnotations;
namespace Hardware_Service_Cetner.Enums;

public enum CustomerStatus
{
    [Display(Name = "Active")]
    Active = 1,
    [Display(Name = "Inactive")]
    Inactive = 0
}
