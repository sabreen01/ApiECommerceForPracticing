namespace MyEcommerce.DTOs;

public class CategorySalesDto
{
    public string CategoryName  { get; set; } =  string.Empty;
    public decimal TotalRevenue   { get; set; } = decimal.Zero;
    public int ItemsSold    { get; set; } = 0;
}