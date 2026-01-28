namespace MyEcommerce.DTOs;

public class SalesByCustomerCityDto
{
    public string? City { get; set; } = string.Empty;
    public int TotalOrders { get; set; } = 0;
    public decimal TotalSpent { get; set; } = 0;
    
}