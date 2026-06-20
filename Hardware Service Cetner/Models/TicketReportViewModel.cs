using Hardware_Service_Cetner.Enums;

namespace Hardware_Service_Cetner.Models;

public class TicketReportViewModel
{
    public int Id { get; set; }
    public string? TicketNo { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int DeviceId { get; set; }
    public string? DeviceName { get; set; }
    public int? TechnicianId { get; set; }
    public string? TechnicianName { get; set; }
    public string? TicketDescription { get; set; }
    public DateTime RecDate { get; set; }
    public int RecById { get; set; }
    public string? ReceivedByName { get; set; }
    public TicketStatus TicketStatus { get; set; }
}
