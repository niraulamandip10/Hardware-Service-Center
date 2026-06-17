namespace Hardware_Service_Cetner.Models;

public class DeviceModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Boolean Status { get; set; } = Boolean.Parse("True");
    
}