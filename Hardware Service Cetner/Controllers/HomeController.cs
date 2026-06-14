using System.Diagnostics;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;

namespace Hardware_Service_Cetner.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly DapperContext _dapperContext;

    public HomeController(ILogger<HomeController> logger, DapperContext dapperContext)
    {
        _logger = logger;
        _dapperContext = dapperContext;
    }

    public async Task<IActionResult> Index()
    {
        using var connection = _dapperContext.CreateConnection();
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