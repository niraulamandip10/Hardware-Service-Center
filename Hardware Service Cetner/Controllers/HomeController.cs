using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;

namespace Hardware_Service_Cetner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public HomeController(ILogger<HomeController> logger, IDbConnectionProvider dbConnectionProvider)
    {
        _logger = logger;
        _dbConnectionProvider = dbConnectionProvider;
    }

    public async Task<IActionResult> Index()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var users = await connection.QueryAsync<AccountModel>("SELECT * FROM users ORDER BY Id DESC");
        return View(users);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}