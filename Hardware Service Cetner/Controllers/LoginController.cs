using Dapper;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class LoginController : Controller
{
    private readonly DapperContext _dapperContext;
    public LoginController(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return View(loginModel);

        using var connection = _dapperContext.CreateConnection();
        var user = connection.QueryFirstOrDefault<AccountModel>(
            "SELECT * FROM users WHERE username = @Username", loginModel);

        if (user == null)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        var passwordHasher = new PasswordHasher<AccountModel>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginModel.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        return RedirectToAction("Index", "Home");
    }
}