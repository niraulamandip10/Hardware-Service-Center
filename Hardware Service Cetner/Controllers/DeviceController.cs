using Dapper;
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

    public async Task<IActionResult> Create(DeviceModel deviceModel)
    {
        if (ModelState.IsValid)
        {
            var connection = _dapperContext.CreateConnection();
            var getdevice = @"Insert into device set (Name,Description) values (@Name,@Description)";
           await connection.ExecuteAsync(getdevice, deviceModel);

            return RedirectToAction("Report");
        }
        return View(deviceModel);
    }

    [HttpGet]
    public IActionResult Report()
    {
        var connection = _dapperContext.CreateConnection();
        var getReport = @"select * from device";
        connection.ExecuteAsync(getReport);
        return View(getReport);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var connection = _dapperContext.CreateConnection();
        var editReport  = await connection.QueryFirstOrDefaultAsync<DeviceModel>("select * from device where Id = @Id", new { Id = id });
        if (editReport != null)
        {
            TempData["Error"] = "Device not found";
            return RedirectToAction("Report");
        }
        return View(editReport);
    }

    [HttpPost]
    public async Task<IActionResult> Edit( int id ,DeviceModel deviceModel)
    {
        var connection = _dapperContext.CreateConnection();
        var editReport = ("update device set Name = @Name , Description =@Description where Id =@Id");
              deviceModel.Id = id;
              await connection.ExecuteAsync(editReport);
              return RedirectToAction("Report");
        
    }
    
}