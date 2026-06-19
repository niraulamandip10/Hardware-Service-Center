using System.Data;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace Hardware_Service_Cetner.Controllers;

public class TicketController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public TicketController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TicketModel ticketModel)
    {
        if (ModelState.IsValid)
        {
            var connection = _dbConnectionProvider.CreateConnection();
            var createTicket = "Insert into "
        }
        
    }
}