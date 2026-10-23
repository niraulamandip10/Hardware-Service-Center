namespace Hardware_Service_Cetner.Models;
using Hardware_Service_Cetner.Enums;

public class LedgerModel
{
    public int id { get; set; }
    public string LedgerName { get; set; }
    public string LedgerCode { get; set; }
    public int CoaLedgerId { get; set; }
    public DateTime RecDate { get; set; }
    public int RecById { get; set; }
    public String Description { get; set; }
    
    public Status Status { get; set; }
    public  COALedger CoaLedger {get; set;}
    
    
    
}