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

    public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
    {
        ViewBag.FluidLayout = true;

        var start = (startDate ?? DateTime.Today.AddDays(-6)).Date;
        var end = (endDate ?? DateTime.Today).Date.AddDays(1);

        var model = await FetchDashboardData(start, end);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboardData(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date.AddDays(1);

        var data = await FetchDashboardData(start, end);
        return Json(data);
    }

    private async Task<DashboardViewModel> FetchDashboardData(DateTime start, DateTime end)
    {
        using var connection = _dbConnectionProvider.CreateConnection();

        var totalTickets = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM tickets WHERE recdate >= @s AND recdate < @e",
            new { s = start, e = end });

        var totalCustomersCount = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(DISTINCT customerid) FROM tickets WHERE recdate >= @s AND recdate < @e",
            new { s = start, e = end });

        var totalRevenue = await connection.QueryFirstOrDefaultAsync<decimal>(
            "SELECT COALESCE(SUM(amount), 0) FROM delevery WHERE recdate >= @s AND recdate < @e AND status = 2",
            new { s = start, e = end });

        var activeTechnicians = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM technician WHERE isactive = true");

        var readyForDelivery = await connection.QueryFirstOrDefaultAsync<int>(
            @"SELECT COUNT(*) FROM tickets t
              WHERE t.ticketstatus = 4
              AND t.recdate >= @s AND t.recdate < @e
              AND NOT EXISTS (SELECT 1 FROM delevery d WHERE d.ticketid = t.id AND d.status = 2)",
            new { s = start, e = end });

        var pendingPayments = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM delevery WHERE status = 1 AND recdate >= @s AND recdate < @e",
            new { s = start, e = end });

        var ticketCounts = await connection.QueryFirstOrDefaultAsync(
            @"select CAST(COUNT(*) FILTER (WHERE ticketstatus IN (1, 2, 3, 4, 5)) AS INTEGER) as CreatedTickets,
       CAST(COUNT(*) FILTER (WHERE ticketstatus = 4) AS INTEGER)                as CompletedTickets,
       CAST(COUNT(*) FILTER (WHERE d.status = 2) AS INTEGER)                    as DeliveredTickets
from tickets t
         join delevery d on t.id = d.ticketid
WHERE t.recdate >= @s
  AND t.recdate < @e",
            new { s = start, e = end });

        int createdCount = ticketCounts?.CreatedTickets ?? 0;
        int completedCount = ticketCounts?.CompletedTickets ?? 0;
        int deliveredCount = ticketCounts?.DeliveredTickets ?? 0;

        var avgRevenue = totalRevenue > 0 && completedCount > 0
            ? totalRevenue / completedCount
            : 0;

        var periodDays = (int)(end - start).TotalDays;
        var prevStart = start.AddDays(-periodDays);
        var prevEnd = start;

        var prevRevenue = await connection.QueryFirstOrDefaultAsync<decimal>(
            "SELECT COALESCE(SUM(amount), 0) FROM delevery WHERE recdate >= @s AND recdate < @e AND status = 2",
            new { s = prevStart, e = prevEnd });

        var revenueGrowth = prevRevenue > 0
            ? Math.Round((totalRevenue - prevRevenue) / prevRevenue * 100, 1)
            : totalRevenue > 0 ? 100 : 0;

        var ticketsTrend = await connection.QueryAsync<TicketsTrendRow>(
            @"WITH date_series AS (
        SELECT generate_series(@s::date, (@e::date - interval '1 day')::date, '1 day')::date as day
      ),
      ticket_counts AS (
        SELECT DATE(recdate) as day,
          CAST(COUNT(*) AS INTEGER) as created_cnt,
          CAST(COUNT(*) FILTER (WHERE ticketstatus = 4) AS INTEGER) as completed_cnt,
          CAST(COUNT(*) FILTER (WHERE ticketstatus = 5) AS INTEGER) as delivered_cnt
        FROM tickets
        WHERE recdate >= @s AND recdate < @e
        GROUP BY DATE(recdate)
      )
      SELECT
          ds.day as day,
          COALESCE(tc.created_cnt, 0) as created,
          COALESCE(tc.completed_cnt, 0) as completed,
          COALESCE(tc.delivered_cnt, 0) as delivered
      FROM date_series ds
      LEFT JOIN ticket_counts tc ON ds.day = tc.day
      ORDER BY ds.day",
            new { s = start, e = end });
        

        var revenueTrend = await connection.QueryAsync<RevenueTrendRow>(
            @"WITH date_series AS (
                SELECT generate_series(@s::date, (@e::date - interval '1 day')::date, '1 day')::date as day
              ),
              revenue_data AS (
                SELECT DATE(recdate) as day, COALESCE(SUM(amount), 0) as rev
                FROM delevery
                WHERE recdate >= @s AND recdate < @e AND status = 2
                GROUP BY DATE(recdate)
              )
              SELECT ds.day, COALESCE(rd.rev, 0) as rev
              FROM date_series ds
              LEFT JOIN revenue_data rd ON ds.day = rd.day
              ORDER BY ds.day",
            new { s = start, e = end });

        var customerTrend = await connection.QueryAsync<CustomerTrendRow>(
            @"WITH date_series AS (
                SELECT generate_series(@s::date, (@e::date - interval '1 day')::date, '1 day')::date as day
              ),
              daily_customers AS (
                SELECT DATE(t.recdate) as day, t.customerid,
                  CASE WHEN EXISTS (SELECT 1 FROM tickets t2 WHERE t2.customerid = t.customerid AND t2.recdate < DATE(t.recdate) AND t2.recdate < @e) THEN 0 ELSE 1 END as is_new
                FROM tickets t
                WHERE t.recdate >= @s AND t.recdate < @e
                GROUP BY DATE(t.recdate), t.customerid, is_new
              ),
              daily_stats AS (
                SELECT day,
                  COUNT(DISTINCT customerid) FILTER (WHERE is_new = 1) as new_cust,
                  COUNT(DISTINCT customerid) FILTER (WHERE is_new = 0) as returning_cust,
                  COUNT(DISTINCT customerid) as total_cust
                FROM daily_customers
                GROUP BY day
              )
              SELECT ds.day, COALESCE(ds2.new_cust, 0) as new_cust,
                     COALESCE(ds2.returning_cust, 0) as returning_cust,
                     COALESCE(ds2.total_cust, 0) as total_cust
              FROM date_series ds
              LEFT JOIN daily_stats ds2 ON ds.day = ds2.day
              ORDER BY ds.day",
            new { s = start, e = end });

        var technicianPerformance = await connection.QueryAsync<TechnicianPerformanceDto>(
            @"SELECT
                tech.name AS TechnicianName,
                COUNT(t.id) AS TotalAssigned,
                COUNT(t.id) FILTER (WHERE t.ticketstatus IN (4, 5)) AS CompletedTickets,
                CASE WHEN COUNT(t.id) > 0
                  THEN ROUND(COUNT(t.id) FILTER (WHERE t.ticketstatus IN (4, 5)) * 100.0 / COUNT(t.id), 1)
                  ELSE 0 END AS CompletionRate,
                COALESCE(SUM(d.amount) FILTER (WHERE d.status = 2), 0) AS RevenueGenerated
              FROM technician tech
              LEFT JOIN tickets t ON t.technicianid = tech.id AND t.recdate >= @s AND t.recdate < @e
              LEFT JOIN delevery d ON d.ticketid = t.id
              WHERE tech.isactive = true
              GROUP BY tech.id, tech.name
              ORDER BY CompletedTickets DESC",
            new { s = start, e = end });

        var newCustomers = await connection.QueryFirstOrDefaultAsync<int>(
            @"SELECT COUNT(DISTINCT t.customerid) FROM tickets t
              WHERE t.recdate >= @s AND t.recdate < @e
              AND NOT EXISTS (SELECT 1 FROM tickets t2 WHERE t2.customerid = t.customerid AND t2.recdate < @s)",
            new { s = start, e = end });

        var returningCustomers = await connection.QueryFirstOrDefaultAsync<int>(
            @"SELECT COUNT(DISTINCT t.customerid) FROM tickets t
              WHERE t.recdate >= @s AND t.recdate < @e
              AND EXISTS (SELECT 1 FROM tickets t2 WHERE t2.customerid = t.customerid AND t2.recdate < @s)",
            new { s = start, e = end });
        
        var ticketsTrendDto = new TicketsTrendDto();

        foreach (var point in ticketsTrend)
        {
            ticketsTrendDto.Labels.Add(point.day.ToString("MMM dd"));
            ticketsTrendDto.Created.Add(point.created);
            ticketsTrendDto.Completed.Add(point.completed);
            ticketsTrendDto.Delivered.Add(point.delivered);
        }

        var revenueTrendDto = new RevenueTrendDto();

        foreach (var point in revenueTrend)
        {
            revenueTrendDto.Labels.Add(point.day.ToString("MMM dd"));
            revenueTrendDto.Revenue.Add(point.revenue);
        }

        var customerTrendDto = new CustomerTrendDto();

        foreach (var point in customerTrend)
        {
            customerTrendDto.Labels.Add(point.day.ToString("MMM dd"));
            customerTrendDto.New.Add(point.newCust);
            customerTrendDto.Returning.Add(point.returningCust);
            customerTrendDto.Total.Add(point.total);
        }

        return new DashboardViewModel
        {
            TotalTickets = totalTickets,
            TotalCustomers = totalCustomersCount,
            TotalRevenue = totalRevenue,
            ActiveTechnicians = activeTechnicians,
            TicketsReadyForDelivery = readyForDelivery,
            PendingPayments = pendingPayments,
            CreatedTickets = createdCount,
            CompletedTickets = completedCount,
            DeliveredTickets = deliveredCount,
            RevenueGrowth = revenueGrowth,
            AverageRevenuePerTicket = Math.Round(avgRevenue, 2),
            NewCustomers = newCustomers,
            ReturningCustomers = returningCustomers,
            TicketsTrend = ticketsTrendDto,
            RevenueTrend = revenueTrendDto,
            CustomerTrend = customerTrendDto,
            TechnicianPerformance = technicianPerformance.ToList()
        };
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
