using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class DeviceController : Controller
{
    private readonly DapperContext _dapperContext;
    public DeviceController(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]

    public Task<IActionResult> Create(DeviceModel deviceModel)
    {
        var connection = _dapperContext.CreateConnection();
        var getdevice = @"Insert into device set ()"
        
    }
    
}