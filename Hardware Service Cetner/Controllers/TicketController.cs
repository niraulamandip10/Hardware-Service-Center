using System.Data;
using Dapper;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Enums;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class TicketController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public TicketController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var getCustomer =
            await connection.QueryAsync<CustomerModel>("select id, name from customer where Status = @Status",
                new { Status = 1 });
        var getDevice = await connection.QueryAsync<DeviceModel>("select id, name from device where Status = @Status",
            new { Status = true });
        var getRecById =
            await connection.QueryAsync<AccountModel>("select id, name from users where isactive = @Status",
                new { Status = true });

        ViewBag.getCustomer = getCustomer;
        ViewBag.getDevice = getDevice;
        ViewBag.getRecById = getRecById;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(TicketModel ticketModel)
    {
        if (!ModelState.IsValid)
        {
            using var connection = _dbConnectionProvider.CreateConnection();
            var getCustomer =
                await connection.QueryAsync<CustomerModel>("select id, name from customer where Status = @Status",
                    new { Status = 1 });
            var getDevice =
                await connection.QueryAsync<DeviceModel>("select id, name from device where Status = @Status",
                    new { Status = true });
            var getRecById =
                await connection.QueryAsync<AccountModel>("select id, name from users where isactive = @Status",
                    new { Status = true });

            ViewBag.getCustomer = getCustomer;
            ViewBag.getDevice = getDevice;
            ViewBag.getRecById = getRecById;

            return View(ticketModel);
        }

        using var conn = _dbConnectionProvider.CreateConnection();

        var lastId = await conn.QueryFirstOrDefaultAsync<int?>(
            "SELECT MAX(Id) FROM tickets WHERE ticketstatus in (1,2,3,4,5)");

        var ticketNo = $"TKT-{DateTime.Now:yyyyMMdd}-{((lastId ?? 0) + 1):D4}";

        var sql = @"INSERT INTO tickets
        (ticketno, customerid, deviceid, ticketdescription, recdate, recbyid, ticketstatus)
        VALUES
        (@TicketNo, @CustomerId, @DeviceId, @TicketDescription, @RecDate, @RecById, @TicketStatus)";

        await conn.ExecuteAsync(sql, new
        {
            TicketNo = ticketNo,
            ticketModel.CustomerId,
            ticketModel.DeviceId,
            ticketModel.TicketDescription,
            RecDate = DateTime.Now,
            ticketModel.RecById,
            TicketStatus = (int)TicketStatus.Pending
        });

        TempData["Success"] = "Ticket created successfully!";
        return RedirectToAction("Report");
    }

    [HttpGet]
    public async Task<IActionResult> Report()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var sql = @"SELECT t.id, t.ticketno, t.customerid, t.deviceid, t.technicianid,
                    t.ticketdescription, t.recdate, t.recbyid, t.ticketstatus,
                    c.name AS customername, d.name AS devicename,
                    u.name AS receivedbyname, tech.name AS technicname
                    FROM tickets t
                    LEFT JOIN customer c ON t.customerid = c.id
                    LEFT JOIN device d ON t.deviceid = d.id
                    LEFT JOIN users u ON t.recbyid = u.id
                    LEFT JOIN technician tech ON t.technicianid = tech.id
                    ORDER BY t.id DESC";
        var tickets = await connection.QueryAsync<TicketReportViewModel>(sql);
        return View(tickets);
    }

    [HttpGet]
    public async Task<IActionResult> Assigned()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var sql = @"SELECT t.id, t.ticketno, t.customerid, t.deviceid, t.technicianid,
                    t.ticketdescription, t.recdate, t.recbyid, t.ticketstatus,
                    c.name AS customername, d.name AS devicename,
                    u.name AS receivedbyname, tech.name AS technicname
                    FROM tickets t
                    LEFT JOIN customer c ON t.customerid = c.id
                    LEFT JOIN device d ON t.deviceid = d.id
                    LEFT JOIN users u ON t.recbyid = u.id
                    INNER JOIN technician tech ON t.technicianid = tech.id
                    WHERE t.ticketstatus = @Status
                    ORDER BY t.id DESC";
        var tickets =
            await connection.QueryAsync<TicketReportViewModel>(sql, new { Status = (int)TicketStatus.Assigned });
        return View(tickets);
    }

    [HttpGet]
    public async Task<IActionResult> Completed()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var sql = @"SELECT t.id, t.ticketno, t.customerid, t.deviceid, t.technicianid,
                    t.ticketdescription, t.recdate, t.recbyid, t.ticketstatus,
                    c.name AS customername, d.name AS devicename,
                    u.name AS receivedbyname, tech.name AS technicname
                    FROM tickets t
                    LEFT JOIN customer c ON t.customerid = c.id
                    LEFT JOIN device d ON t.deviceid = d.id
                    LEFT JOIN users u ON t.recbyid = u.id
                    LEFT JOIN technician tech ON t.technicianid = tech.id
                    WHERE t.ticketstatus = 4
                    ORDER BY t.id DESC";
        var tickets = await connection.QueryAsync<TicketReportViewModel>(sql,
            new { Statuses = new[] { (int)TicketStatus.Completed, (int)TicketStatus.Delevered } });
        return View(tickets);
    }

    [HttpGet]
    public async Task<IActionResult> Assign(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();

        var sql = @"SELECT t.id, t.ticketno, t.customerid, t.deviceid, t.technicianid,
                    t.ticketdescription, t.recdate, t.recbyid, t.ticketstatus,
                    c.name AS customername, d.name AS devicename,
                    u.name AS receivedbyname, tech.name AS technicname
                    FROM tickets t
                    LEFT JOIN customer c ON t.customerid = c.id
                    LEFT JOIN device d ON t.deviceid = d.id
                    LEFT JOIN users u ON t.recbyid = u.id
                    LEFT JOIN technician tech ON t.technicianid = tech.id
                    WHERE t.id = @Id";
        var ticket = await connection.QueryFirstOrDefaultAsync<TicketReportViewModel>(sql, new { Id = id });

        if (ticket == null)
        {
            TempData["Error"] = "Ticket not found.";
            return RedirectToAction("Report");
        }

        var technicians = await connection.QueryAsync<TechnicianModel>(
            "SELECT id, name FROM technician WHERE isactive = @Status", new { Status = true });

        ViewBag.Technicians = technicians;
        return View(ticket);
    }

    [HttpPost]
    public async Task<IActionResult> Assign(int id, int technicianId, int ticketStatus)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE tickets SET technicianid = @TechnicianId, ticketstatus = @TicketStatus WHERE id = @Id",
            new { Id = id, TechnicianId = technicianId, TicketStatus = ticketStatus });

        TempData["Success"] = "Ticket assigned successfully!";
        return RedirectToAction("Report");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, int ticketStatus)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        await connection.ExecuteAsync(
            "UPDATE tickets SET ticketstatus = @TicketStatus WHERE id = @Id",
            new { Id = id, TicketStatus = ticketStatus });

        var statusName = Enum.GetName(typeof(TicketStatus), ticketStatus);
        TempData["Success"] = $"Ticket status updated to {statusName}!";
        return RedirectToAction("Report");
    }


    [HttpPost]
    public async Task<IActionResult> Repair(int id, int ticketStatus)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        await connection.ExecuteAsync("Update tickets set ticketstatus = @TicketStatus where id = @Id",
            new { Id = id, TicketStatus = ticketStatus });
        return RedirectToAction("Report");
    }

    [HttpGet]
    public async Task<IActionResult> Complete(int id, int ticketStatus)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var completeStatus = await connection.QueryFirstAsync<TicketModel>("Select * from  tickets where Id = @Id and  ticketstatus = @TicketStatus",
            new { Id = id, TicketStatus = (int)TicketStatus.Repairing });

        if (completeStatus == null)
        {
            TempData["Error"] = "Ticket not found.";
            return RedirectToAction("Report");
        }
        
        return View(completeStatus);
    }

    [HttpPost]
    public async Task<IActionResult> Complete(int id, TicketModel ticketModel , DeliveryModel deliveryModel)
    {
        using var connection = _dbConnectionProvider.CreateConnection();

        if (ModelState.IsValid)
        {
            var complete = "Insert into delevery set (TicketId,UserId,RecDate,Amount , Remarks) values (@Id,@UserId,@RecDate,@Amount , @Remarks)";
            await connection.ExecuteAsync(complete, ticketModel);
            var updatestatus = "Update  tickets set ticketstatus = @TicketStatus where id = @Id" ;
            await connection.ExecuteAsync(updatestatus, new {TicketStatus = (int)TicketStatus.Completed});
        }
        return RedirectToAction("Report");
        
    }

    [HttpGet]
    public async Task<IActionResult> Reverse(int id)
    {
        using var connection = _dbConnectionProvider.CreateConnection();

        if (TicketModel.TicketStatus == TicketStatus.Completed)
        {
            TempData["Error"] = "Ticket status cannot be changed once completed.";
            return RedirectToAction("Report");
        }
        var reversetkt = connection.QueryFirstOrDefaultAsync<TicketModel>("Select * from tickets where Id = @Id");

        return View(reversetkt);
    }

    [HttpPost]
    public async Task<IActionResult> Reverse(int id, TicketModel ticketModel)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var reverse = "Update tickets Set Status = @Status where Id = @Id and TicketStatus != 4";
        connection.ExecuteAsync(reverse, ticketModel.Id = id);
        return RedirectToAction("Report");
    }
}