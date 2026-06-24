using Hardware_Service_Cetner.Enums;

namespace Hardware_Service_Cetner.Models;

public class DeleveryModel
{
    public int Id { get; set; }
    public int Ticket_id  { get; set; }
    public int User_id { get; set; }
    public DateTime RecDate { get; set; }
    public double Amount { get; set; }
    public string PaymentMethod { get; set; }
    public DeliveryStatus Status { get; set; } 
    public string Remarks { get; set; }
    
    
        
    
}