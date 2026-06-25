using Dapper;
using Hardware_Service_Cetner.Data;
using Hardware_Service_Cetner.Enums;
using Hardware_Service_Cetner.Models;
using Microsoft.AspNetCore.Mvc;

namespace Hardware_Service_Cetner.Controllers;

public class DeliveryController : Controller
{
    private readonly IDbConnectionProvider _dbConnectionProvider;

    public DeliveryController(IDbConnectionProvider dbConnectionProvider)
    {
        _dbConnectionProvider = dbConnectionProvider;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var sql = @"SELECT t.id, t.ticketno, t.ticketdescription, t.recdate,
                    c.name AS customername, d.name AS devicename,
                    tech.name AS technicname
                    FROM tickets t
                    LEFT JOIN customer c ON t.customerid = c.id
                    LEFT JOIN device d ON t.deviceid = d.id
                    LEFT JOIN technician tech ON t.technicianid = tech.id
                    WHERE t.ticketstatus = @Completed
                    AND t.id NOT IN (
                        SELECT ticketid FROM delevery WHERE status = @Delivered
                    )
                    ORDER BY t.id DESC";
        var tickets = await connection.QueryAsync<TicketReportViewModel>(sql, new
        {
            Completed = (int)TicketStatus.Completed,
            Delivered = (int)DeliveryStatus.delivered
        });
        return View(tickets);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(int ticketId)
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var ticket = await connection.QueryFirstOrDefaultAsync<TicketReportViewModel>(
            @"SELECT t.id, t.ticketno, t.customerid, t.deviceid, t.technicianid,
              t.ticketdescription, t.recdate, t.recbyid, t.ticketstatus,
              c.name AS customername, d.name AS devicename,
              u.name AS receivedbyname, tech.name AS technicname
              FROM tickets t
              LEFT JOIN customer c ON t.customerid = c.id
              LEFT JOIN device d ON t.deviceid = d.id
              LEFT JOIN users u ON t.recbyid = u.id
              LEFT JOIN technician tech ON t.technicianid = tech.id
              WHERE t.id = @Id", new { Id = ticketId });

        if (ticket == null)
        {
            TempData["Error"] = "Ticket not found.";
            return RedirectToAction("Index");
        }

        var existingDelivery = await connection.QueryFirstOrDefaultAsync<DeliveryModel>(
            "SELECT * FROM delevery WHERE ticketid = @TicketId", new { TicketId = ticketId });

        if (existingDelivery != null)
        {
            return View("Checkout", existingDelivery);
        }

        return View("Checkout", new DeliveryModel { TicketId = ticketId });
    }

    [HttpPost]
    public async Task<IActionResult> Checkout(DeliveryModel deliveryModel)
    {
        using var connection = _dbConnectionProvider.CreateConnection();

        if (!ModelState.IsValid)
            return View(deliveryModel);

        var sql = @"
        UPDATE delevery
        SET paymentmethod = @PaymentMethod,
            remarks = @Remarks,
            status = @Status
        WHERE TicketId = @TicketId";
        deliveryModel.Status = DeliveryStatus.delivered;

        var rows = await connection.ExecuteAsync(sql, deliveryModel);

        TempData["Success"] = "Delivery completed successfully!";
        return RedirectToAction("Report");
    }

    [HttpGet]
    public async Task<IActionResult> Report()
    {
        using var connection = _dbConnectionProvider.CreateConnection();
        var sql = @"SELECT d.id, d.ticketid, d.userid, d.recdate, d.amount,
                    d.paymentmethod, d.status, d.remarks,
                    t.ticketno, c.name AS customername, dev.name AS devicename,
                    tech.name AS technicname, u.name AS username
                    FROM delevery d
                    INNER JOIN tickets t ON d.ticketid = t.id
                    LEFT JOIN customer c ON t.customerid = c.id
                    LEFT JOIN device dev ON t.deviceid = dev.id
                    LEFT JOIN technician tech ON t.technicianid = tech.id
                    LEFT JOIN users u ON d.userid = u.id
                    ORDER BY d.id DESC";
        var result = await connection.QueryAsync<DeliveryReportViewModel>(sql);
        return View(result);
    }
}