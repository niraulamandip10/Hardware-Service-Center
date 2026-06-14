using System.ComponentModel.DataAnnotations;
using Hardware_Service_Cetner.Enums;
namespace Hardware_Service_Cetner.Models;

public class TechnicianModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Code is required")]
    [Display(Name = "Technician Code")]
    public int Code { get; set; }

    [Display(Name = "Registration Date")]
    public DateTime RecDate { get; set; } = DateTime.UtcNow;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;
}