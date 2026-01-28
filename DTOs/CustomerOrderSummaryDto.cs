namespace MyEcommerce.DTOs;

public class CustomerOrderSummaryDto
{
    public string CustomerName { get; set; } =  string.Empty;
    public int TotalOrders { get; set; } = 0;
    public decimal TotalSpent { get; set; }
    
}