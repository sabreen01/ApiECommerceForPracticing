using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEcommerce.DTOs;
using MyEcommerce.Models;

namespace MyEcommerce.Controllers;



[Route("api/[controller]")]
[ApiController]
public class CategoriesController(EcommerceContext context)
{
    [HttpGet("Sales-Reports")]
    public async Task<ActionResult<IEnumerable<CategorySalesDto>>> GetCategorySalesReport()
    {
        var report = await context.OrderItems
            .AsNoTracking() 
            
            .GroupBy(oi => oi.Product.Category.Name) 
            .Select(g => new CategorySalesDto
            {
              
                CategoryName = g.Key ?? "Uncategorized", 
                
                ItemsSold = g.Sum(oi => oi.Quantity),
                
                TotalRevenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .ToListAsync();
        return report;
    }


    [HttpGet("CategoriesOutOfStock")]
    public async Task<ActionResult<IEnumerable<string>>> GetCategoriesOutOfStock()
    {
        var category = await context.Categories
            .Where(c => c.Products.All(p => p.Stockquantity == 0))
            .Select(c => c.Name)
            .ToListAsync();
        return category;
    }

    [HttpGet("category-active-products/{categoryId}")]
    public async Task<ActionResult<object>> GetCategoriesActiveProducts(int categoryId)
    {
        var category = await context.Categories
            .Include(c => c.Products.Where(p => p.Stockquantity > 0))
            .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        return new
        {
            category?.Name,
            ActiveProducts = category?.Products.Select(p => p.Name).ToList()
        };
    }
    
    
    //update
    [HttpPatch("UpdateCategoryPrices/{categoryId}")]
    public async Task<ActionResult<object>> UpdateCategoryPrices( int categoryId)
    {
        decimal percentage = 1.1m;

        var affectedRows = await context.Database.ExecuteSqlAsync($@"
        UPDATE products 
        SET Price = Price * {percentage} 
        WHERE ""category_id"" = {categoryId} 
        ");

        return new { Message = "Successfully updated category", AffectedRows = affectedRows };
    }
    
    
}