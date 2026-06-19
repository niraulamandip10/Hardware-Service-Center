using Hardware_Service_Cetner.Enums;

namespace Hardware_Service_Cetner.Models;

public class TicketModel
{
    public int Id { get; set; }
    public int TicketNo { get; set; }
    public int CustomerId { get; set; }
    public int DeviceId { get; set; }
    public int TechnicianId { get; set; }
    public string TicketDescription { get; set; }
    public DateTime RecDate { get; set; }
    public int RecById { get; set; }
    public TicketStatus TicketStatus { get; set; }
   
    
    
}