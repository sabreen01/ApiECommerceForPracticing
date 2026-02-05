
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEcommerce.DTOs;
using MyEcommerce.Models;
using MyEcommerce.Services.Interfaces;

namespace MyEcommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportsController(IReportService reportService,EcommerceContext context) : ControllerBase
{
    

    private static readonly Func<EcommerceContext, int, IAsyncEnumerable<Product>> GetProductsByCategory =
        EF.CompileAsyncQuery((EcommerceContext context, int categoryId) =>
            context.Products.Where(p => p.CategoryId == categoryId));
    

    // private static readonly Func<EcommerceContext, string, Task<Customer?>> GetCustomerByEmail =
    //     EF.CompileAsyncQuery((EcommerceContext context, string email) =>
    //         context.Customers.FirstOrDefault(c => c.Email == email));
    //
    //
    // private static readonly Func<EcommerceContext, int, Task<Order?>> GetOrderSummary =
    //     EF.CompileAsync Query((EcommerceContext context, int orderId) =>
    //         context.Orders
    //             .Include(o => o.OrderItems)
    //             .FirstOrDefault(o => o.OrderId == orderId));



    [HttpGet("CityAnalysis")]
    public async Task<ActionResult<IEnumerable<SalesByCustomerCityDto>>> GetCityAnalysis()
    {
        var data = await reportService.GetCityAnalysis();
        return Ok(data);
        // var report = await context.Orders
        //     .GroupBy(o=> o.City)
        //     .Select(g => new SalesByCustomerCityDto
        //     {
        //         City = g.Key,
        //         TotalOrders =  g.Count(),
        //         TotalSpent = g.SelectMany(o => o.OrderItems)
        //                 .Sum(o =>o.UnitPrice * o.Quantity)
        //         
        //     })
        //     .ToListAsync();
        // return report;
        
    }

    [HttpGet("MonthAnalysisForSalse")]
    public async Task<ActionResult<IEnumerable<MonthlySalesDto>>> GetMonthlySalesForSalse()
    {
        var data = await reportService.GetMonthlySalesForSalse();
        return Ok(data);
        
    }

    [HttpGet("Top-3-product-per-category")]
    public async Task<ActionResult<IEnumerable<TopProductsPerCategoryDto>>> GetTop3ProductPerCategory()
    {

        var data = reportService.GetTop3ProductPerCategory();
        return Ok(data);



        // var report = await context.Products
        //     .Select(p => new TopProductsPerCategoryDto
        //     {
        //         CategoryName = p.Category.Name,
        //         TopProducts = p.OrderItems
        //             .GroupBy(g => g.ProductId)
        //             .Select(oi => new ProductRevenue
        //             {
        //                 ProductName = p.Name,
        //                 Revenue = oi.Sum(s => s.Quantity * s.UnitPrice)
        //             })
        //             .OrderByDescending(r => r.Revenue)
        //             .Take(3)
        //             .ToList()
        //     })
        //     // .Distinct()
        //     .ToListAsync();
        //
        // var uniqueReport = report
        //     .GroupBy(r => r.CategoryName)
        //     .Select(g => g.First())
        //     .ToList();
        // return uniqueReport;

    }

    [HttpGet("GetTotalRevenue")]
    public async Task<ActionResult<object>> GetTotalRevenue()
    {
        var data = await reportService.GetTotalRevenue();
        return data;
        

    }

    [HttpGet("ProductSales/{id}")]
    public async Task<ActionResult<object>> GetSales(int id)
    {
        var data = await reportService.GetSales(id);
        return data;

    }


    [HttpGet("AllPayments")]
    public async Task<ActionResult<object>> GetAllPayments()
    {
        var payments = await context.Payments.ToListAsync();
        var result = payments.Select(p => new
        {
            p.Id,
            p.Amount,
            Type = p switch
            {
                CreditCardPayment => "Credit Card",
                PayPalPayment => "PayPal",
                _ => "Unknown"
            },

            Details = p switch
            {
                CreditCardPayment c => $"Card: {c.Cardholdername}",
                PayPalPayment py => $"Email: {py.Email}",
                _ => "No details"
            }
        });

        return result.ToList();
    }

    [HttpGet("ByCategory/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {

        var products = GetProductsByCategory(context, categoryId);
        return Ok(await products.ToListAsync());
    }

}