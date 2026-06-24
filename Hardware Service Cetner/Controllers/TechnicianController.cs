using Dapper;
using Hardware_Service_Cetner.Data;
using Microsoft.AspNetCore.Mvc;
using Hardware_Service_Cetner.Models;
namespace Hardware_Service_Cetner.Controllers;


public class TechnicianController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public TechnicianController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TechnicianModel technician)
    {
      
        
      if (ModelState.IsValid)
        {
            technician.IsActive = true;
            technician.RecDate = DateTime.UtcNow;
            using var connection = _dbConnectionProvider.CreateConnection();
            {
                var createtech = @"Insert into technician (Name, Description, Code, RecDate, IsActive) values (@Name, @Description, @Code, @RecDate, @IsActive)";
                await connection.ExecuteAsync(createtech, technician);
                return RedirectToAction("Report");
            }
            
        }
        return View(technician);
        
    }

    [HttpGet]
    public async Task<IActionResult> Report()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var technician = await connection.QueryAsync<TechnicianModel>(@"Select * from technician order by id desc");

        return View(technician);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var technician = await connection.QueryFirstOrDefaultAsync<TechnicianModel>("Select * from technician where id = @id", new { id });
        if (technician == null)
            return NotFound();
        return View(technician);
    }

    [HttpPost]
    public async Task<IActionResult> Edit( int id ,TechnicianModel technician)
    {
        if (ModelState.IsValid)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            var edit = @"update technician set Name = @Name, Description = @Description, Code = @Code where id = @id";
            technician.Id = id;
            await connection.ExecuteAsync(edit, technician);
            return RedirectToAction("Report");
        }

        return View(technician);
    }

    [HttpPost]
    public IActionResult Delete(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var delete = @"Delete from technician where Id = @id";
        connection.Execute(delete, new { id });
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var activate = @"Update technician set IsActive = @IsActive WHERE id = @id";
        await connection.ExecuteAsync(activate, new { IsActive = true, id });
        TempData["Success"] = "Technician activated successfully!";
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var deactivate = @"Update technician set IsActive = @IsActive WHERE id = @id";
        await connection.ExecuteAsync(deactivate, new { IsActive = false, id });
        TempData["Success"] = "Technician deactivated successfully!";
        return RedirectToAction("Report");
    }
}