using Dapper;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class DeviceController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    public DeviceController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
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
            var connection = _dbConnectionProvider.CreateConnection();
            var getdevice = @"Insert into device  (Name,Description) values (@Name,@Description)";
           await connection.ExecuteAsync(getdevice, deviceModel);

            return RedirectToAction("Report");
        }
        return View(deviceModel);
    }

    [HttpGet]
    public async Task<IActionResult> Report()
    {
        var connection = _dbConnectionProvider.CreateConnection();
        var getReport = await  connection.QueryAsync<DeviceModel>("select * from device  order by Id desc");
        return View(getReport);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var connection = _dbConnectionProvider.CreateConnection();
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
        if (ModelState.IsValid)
        {
            var connection = _dbConnectionProvider.CreateConnection();
            var editReport = ("update device set Name = @Name , Description =@Description where Id =@Id");
            deviceModel.Id = id;
            await connection.ExecuteAsync(editReport);
            return RedirectToAction("Report");
        }
        return View(deviceModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var connection = _dbConnectionProvider.CreateConnection();
        var deletedev = ("Delete from device where  Id = @Id");
        await connection.ExecuteAsync(deletedev, new {  id });
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        var connection = _dbConnectionProvider.CreateConnection();
        var activateRep = ("Update device set Status = @Status where Id = @Id");
        await connection.ExecuteAsync(activateRep,  new { Status = Boolean.Parse("True"), id });
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        var connection = _dbConnectionProvider.CreateConnection();
        var deactiveRep = ("Update device set Status = @Status where Id = @Id");
        await connection.ExecuteAsync(deactiveRep,  new { Status = Boolean.Parse("False"), id });
        return RedirectToAction("Report");
    }

}