namespace MyEcommerce.DTOs;

public class TopProductsPerCategoryDto
{
    public string CategoryName { get; set; }
    public List<ProductRevenue> TopProducts{get;set;}
}
public class ProductRevenue
{
    public string ProductName { get; set; }
    public decimal Revenue { get; set; }
}