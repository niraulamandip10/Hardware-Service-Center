using System.Security.Claims;
using Dapper;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class LoginController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;
    public LoginController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginModel loginModel)
    {
        if (!ModelState.IsValid)
            return View(loginModel);

        using var connection = _dbConnectionProvider.CreateConnection();
        var user = connection.QueryFirstOrDefault<AccountModel>(
            "SELECT * FROM users WHERE username = @Username", loginModel);

        if (user == null)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        if (!user.IsActive)
        {
            ViewBag.Error = "Your account has been deactivated. Contact an administrator.";
            return View();
        }

        var passwordHasher = new PasswordHasher<AccountModel>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginModel.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ViewBag.Error = "Invalid username or password";
            return View();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("Username", user.Username)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return RedirectToAction("Index", "Home");
    }
}