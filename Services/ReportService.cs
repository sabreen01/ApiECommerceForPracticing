using Microsoft.EntityFrameworkCore;
using MyEcommerce.DTOs;
using MyEcommerce.Interfaces;
using MyEcommerce.Models;

namespace MyEcommerce.Services;

public class ReportService : IReportService
{
    
    private readonly EcommerceContext context;
    public ReportService(EcommerceContext context)
    {
        this.context = context;
    }
    public async Task<IEnumerable<TopProductsPerCategoryDto>> GetTop3ProductPerCategory()
    {
        var report = await context.Categories
            .AsNoTracking()
            .Select(c => new TopProductsPerCategoryDto
            {
                CategoryName = c.Name,
                TopProducts = c.Products
                    .Select(p => new ProductRevenue
                    {
                        ProductName = p.Name,
                        Revenue = p.OrderItems
                            .Sum(oi => oi.Quantity * oi.UnitPrice)
                    })
                    .OrderByDescending(p => p.Revenue)
                    .Take(3)
                    .ToList()
            })
            .ToListAsync(); 
        return report;
        
    }

    public async Task<IEnumerable<SalesByCustomerCityDto>> GetCityAnalysis()
    {
        var report = await context.Orders
            .GroupBy(o => o.Address.City)
            .Select(g => new SalesByCustomerCityDto
            {
                City = g.Key,
                TotalOrders = g.Count(),
                TotalSpent = g.SelectMany(o => o.OrderItems)
                    .Sum(o => o.UnitPrice * o.Quantity)
            })
            .ToListAsync();
        return report;
    }

    public async Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesForSalse()
    {
        var report = await context.Orders
            .GroupBy(o => new
            {
                year = o.OrderDate.Value.Year,
                month = o.OrderDate.Value.Month
            })
            .Select(g => new MonthlySalesDto
            {
                Year = g.Key.year,
                Month = g.Key.month,

                TotalRevenue = g.SelectMany(o => o.OrderItems)
                    .Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .ToListAsync();

        return report;
    }

    public async Task<object> GetTotalRevenue()
    {
        var result = await context.RevenueResults
            .FromSqlRaw("select * from get_total_revenue()")
            .ToListAsync();

        var total = result.FirstOrDefault()?.TotalRevenue ?? 0;
        return new { TotalRevenue = total };
    }

    public async Task<object> GetSales(int id)
    {
       
        var result = await context.RevenueResults
            .FromSqlRaw("SELECT * FROM get_product_sales({0})", id)
            .ToListAsync();

        var total = result.FirstOrDefault()?.TotalRevenue ?? 0;

        return new { ProductId = id, Sales = total };
    }

  
    
}