using Dapper;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hardware_Service_Cetner.Controllers;

[AllowAnonymous]
public class LoginController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public LoginController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        ViewData["ReturnUrl"] = returnUrl;
        ViewBag.CanRegister = !await HasActiveUsersAsync();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel loginModel, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        ViewBag.CanRegister = !await HasActiveUsersAsync();

        if (!ModelState.IsValid)
            return View(loginModel);

        using var connection = _dbConnectionProvider.CreateConnection();
        var user = connection.QueryFirstOrDefault<AccountModel>(
            "SELECT * FROM users WHERE username = @Username", loginModel);

        if (user == null)
        {
            ViewBag.Error = "Invalid username or password";
            return View(loginModel);
        }

        var passwordHasher = new PasswordHasher<AccountModel>();
        var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginModel.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            ViewBag.Error = "Invalid username or password";
            return View(loginModel);
        }

        if (!user.IsActive)
        {
            ViewBag.Error = "This account is inactive.";
            return View(loginModel);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new("FullName", user.Name)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = false,
                AllowRefresh = true
            });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private async Task<bool> HasActiveUsersAsync()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>("SELECT EXISTS (SELECT 1 FROM users WHERE isactive = true)");
    }
}
