using Hardware_Service_Cetner.Data;
using Dapper;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class AccountController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public AccountController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Create()
    {
        if (!await CanCreateAccountAsync())
            return Challenge();

        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AccountModel accountModel)
    {
        if (!await CanCreateAccountAsync())
            return Challenge();

        if (ModelState.IsValid)
        {
            var passwordHasher = new PasswordHasher<AccountModel>();
           accountModel.Password = passwordHasher.HashPassword(accountModel, accountModel.Password);
           accountModel.RegistrationDate = DateTime.UtcNow;
           accountModel.IsActive = true;
            
            using var connection = _dbConnectionProvider.CreateConnection();
            var createuser = "INSERT INTO users (Name, Email, Phone, Address, Username, Password, RegistrationDate,IsActive) VALUES (@Name, @Email, @Phone, @Address, @Username, @Password, @RegistrationDate,@IsActive)";
            await connection.ExecuteAsync(createuser, accountModel);
            TempData["Success"] = "Account created successfully!";
            return RedirectToAction("Report");
        }

        return View(accountModel);
    }

    private async Task<bool> CanCreateAccountAsync()
    {
        if (User.Identity?.IsAuthenticated == true)
            return true;

        return !await HasActiveUsersAsync();
    }

    private async Task<bool> HasActiveUsersAsync()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        return await connection.ExecuteScalarAsync<bool>("SELECT EXISTS (SELECT 1 FROM users WHERE isactive = true)");
    }

    [HttpGet]
    public async Task<IActionResult> Report()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var users = await connection.QueryAsync<AccountModel>("SELECT * FROM users ORDER BY Id DESC");
        return View(users);
    }


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var user = await connection.QueryFirstOrDefaultAsync<AccountModel>("SELECT * FROM users WHERE Id = @Id", new { Id = id });
        if (user == null)
            return NotFound();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, AccountModel accountModel)
    {
        if (ModelState.IsValid)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            var updUser = "UPDATE users SET Name=@Name, Email=@Email, Phone=@Phone, Address=@Address, Username=@Username WHERE Id=@Id";
            accountModel.Id = id;
            await connection.ExecuteAsync(updUser, accountModel);
            TempData["Success"] = "Account updated successfully!";
            return RedirectToAction("Report");
        }
        return View(accountModel);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var dltUser = ("Delete from users where id =@id " );
        await connection.ExecuteAsync(dltUser, new { Id = id });
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var activateUser = "Update users set IsActive = @IsActive where Id = @Id";
        await  connection.ExecuteAsync(activateUser, new {IsActive = true, Id = id });
        TempData["Success"] = "Account activated successfully!";
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var deactUser = "Update users set IsActive = @IsActive where Id = @id";
        await connection.ExecuteAsync(deactUser, new { IsActive = false, Id = id });
        TempData["Success"] = "Account deactivated successfully!";
        return RedirectToAction("Report");
    }
    
}
