namespace Hardware_Service_Cetner.Models;

public class DashboardViewModel
{
    public int TotalTickets { get; set; }
    public int TotalCustomers { get; set; }
    public decimal TotalRevenue { get; set; }
    public int ActiveTechnicians { get; set; }
    public int TicketsReadyForDelivery { get; set; }
    public int PendingPayments { get; set; }

    public int CreatedTickets { get; set; }
    public int CompletedTickets { get; set; }
    public int DeliveredTickets { get; set; }
    public decimal RevenueGrowth { get; set; }
    public decimal AverageRevenuePerTicket { get; set; }
    public int NewCustomers { get; set; }
    public int ReturningCustomers { get; set; }

    public TicketsTrendDto TicketsTrend { get; set; } = new();
    public RevenueTrendDto RevenueTrend { get; set; } = new();
    public CustomerTrendDto CustomerTrend { get; set; } = new();
    public List<TechnicianPerformanceDto> TechnicianPerformance { get; set; } = new();
}

public class TicketsTrendDto
{
    public List<string> Labels { get; set; } = new();
    public List<int> Created { get; set; } = new();
    public List<int> Completed { get; set; } = new();
    public List<int> Delivered { get; set; } = new();
}

public class RevenueTrendDto
{
    public List<string> Labels { get; set; } = new();
    public List<decimal> Revenue { get; set; } = new();
}

public class CustomerTrendDto
{
    public List<string> Labels { get; set; } = new();
    public List<int> New { get; set; } = new();
    public List<int> Returning { get; set; } = new();
    public List<int> Total { get; set; } = new();
}


public class TicketsTrendRow
{
    public DateOnly day { get; set; }
    public int created { get; set; }
    public int completed { get; set; }
    public int delivered { get; set; }
}
public class RevenueTrendRow
{
    public DateOnly day { get; set; }
    public decimal revenue { get; set; }
}
public class CustomerTrendRow
{
    public DateOnly day { get; set; }
    public int newCust { get; set; }
    public int returningCust { get; set; }
    public int total { get; set; }
}

public class TechnicianPerformanceDto
{
    public string TechnicianName { get; set; } = "";
    public int TotalAssigned { get; set; }
    public int CompletedTickets { get; set; }
    public decimal CompletionRate { get; set; }
    public decimal RevenueGenerated { get; set; }
}
