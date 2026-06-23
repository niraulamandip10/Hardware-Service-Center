using Dapper;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class DeleveryController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public DeleveryController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(string ticketNo)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var gettkt = @"select t.ticketno as TicketNo , d.* from delevery d inner join public.tickets t on t.id = d.ticketid  where t.TicketNo =@TicketNo  ";
        var result = await connection.ExecuteAsync(gettkt, new
        {
            TicketNo = ticketNo
        });
        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(DeleveryModel deleveryModel)
    {
        using var connection = _dbConnectionProvider.CreateConnection();

        if (ModelState.IsValid)
        {
            var completeDlvry = "Update delevery d set PaymentMethod = @PaymentMethod , RecDate = @RecDate , Status = @Status ,Remarks =@Remarks where Id = @Id";
            await connection.ExecuteAsync(completeDlvry, deleveryModel);
            return RedirectToAction("Report");
        }
        return View(deleveryModel);
    }

    [HttpGet]
    public IActionResult Report()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var getReport = "Select * from delevery ";
        var result = connection.Query<DeleveryModel>(getReport);
        return View(result);
    }
}