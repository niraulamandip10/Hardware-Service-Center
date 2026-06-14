using Hardware_Service_Cetner.Enums;
using Hardware_Service_Cetner.Models;

namespace Hardware_Service_Cetner.Controllers;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Hardware_Service_Cetner.Data;


public class CustomerController : Controller
{
    private readonly DapperContext _dapperContext;
    public CustomerController(DapperContext dapperContext)
    {
        _dapperContext = dapperContext;
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CustomerModel customer)
    {
        if (ModelState.IsValid)
        {
            using var connection = _dapperContext.CreateConnection();
            var createcustomer = @"INSERT INTO customer (Name, Email, Phone, Address, Status) VALUES (@Name, @Email, @Phone, @Address, @Status)";
            await connection.ExecuteAsync(createcustomer, customer);
            TempData["Success"] = "Customer created successfully!";
            return RedirectToAction("Report");
        }
        return View(customer);
    }
    
    
    [HttpGet]
    public async Task<IActionResult> Report()
    {
        using var connection = _dapperContext.CreateConnection();
        var customers = await connection.QueryAsync<CustomerModel>("SELECT * FROM customer ORDER BY Id DESC");
        return View(customers);
    }
    
    
    


    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        using var connection = _dapperContext.CreateConnection();
        var customer = await connection.QueryFirstOrDefaultAsync<CustomerModel>("select * from customer where id = @id", new { id });
        if (customer == null)
        {
            TempData["Error"] = "Customer not found.";
            return RedirectToAction("Report");
        }
        return View(customer);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(int id, CustomerModel customer)
    {
        if (ModelState.IsValid)
        {
            using var connection = _dapperContext.CreateConnection();
            var edit =
                @"UPDATE customer set Name = @Name, Email = @Email, Phone = @Phone, Address = @Address, Status = @Status where id = @Id";
            customer.Id = id;
            await connection.ExecuteAsync(edit, customer);
            TempData["Success"] = "Customer updated successfully!";
            return RedirectToAction("Report");
        }
        return View(customer);
    }


    [HttpPost]
    public IActionResult Delete(int id)
    {
        using var connection = _dapperContext.CreateConnection();
        var delete =@"Delete from customer where  id = @id";
        connection.Execute(delete, new { id });
        return RedirectToAction("Report");
        
    }

    [HttpPost]
    public async Task<IActionResult> Activate(int id)
    {
        using var connection = _dapperContext.CreateConnection();
        var activate = (@"UPDATE customer SET Status = @Status WHERE id = @id");
        connection.Execute(activate, new { CustomerStatus = CustomerStatus.Active });
        TempData["Success"] = "Customer activated successfully!";
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> Deactivate(int id)
    {
        using var connection = _dapperContext.CreateConnection();
        var deactivate = (@"UPDATE customer SET Status = @Status WHERE id = @id");
        connection.Execute(deactivate, new { CustomerStatus = CustomerStatus.Inactive });
        TempData["Success"] = "Customer deactivated successfully!";
        return RedirectToAction("Report");
    }
}