using Hardware_Service_Cetner.Enums;

namespace Hardware_Service_Cetner.Models;

public class DeliveryReportViewModel
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public string? TicketNo { get; set; }
    public string? CustomerName { get; set; }
    public string? DeviceName { get; set; }
    public string? TechnicianName { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime RecDate { get; set; }
    public double Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public DeliveryStatus Status { get; set; }
    public string? Remarks { get; set; }
}
