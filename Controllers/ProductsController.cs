using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEcommerce.Models;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using MyEcommerce.DTOs;

namespace MyEcommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController(EcommerceContext context) : ControllerBase
{
    // GET: api/Products
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
      
        return await context.Products
            .Include(p => p.Category)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
    
    // GET: api/Products/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
      
        var product = await context.Products
            .Include(p => p.Category) 
            .FirstOrDefaultAsync(p => p.ProductId == id);

        if (product == null)
        {
            return NotFound(); 
        }

        return product;
    }
    
    [HttpGet("never-ordered-products")]
    public async Task<ActionResult<IEnumerable<string>>> GetNeverOrderedProducts()
    {
        var products = await context.Products
            .Where(p => p.OrderItems.Count() == 0) 
            .Select(p => p.Name)
            .ToListAsync();

        return products;
    }

    [HttpPost("orders-by-products-count")]
    public async Task<ActionResult<int>> GetOrdersByProductsCount(List<int> IDs)
    {
       
        var ordersCount = await context.OrderItems
            .Where(oi => IDs.Contains(oi.ProductId))
            .GroupBy(oi => oi.OrderId)
            .CountAsync(); 

        return ordersCount ;
    }
    
    
    // [HttpPost("orders-by-products")]
    // public async Task<ActionResult<int>> GetOrdersByProducts(List<int> IDs)
    // {
    //     var order = await context.OrderItems
    //         .Where(oi => IDs.Contains(oi.ProductId))
    //         .GroupBy(oi => oi.OrderId)
    //         .Select(g => g.Count())
    //         .FirstOrDefaultAsync();
    //     return order;
    // } 
    
    [HttpGet("OrderedProducts")]
    public async Task<ActionResult> GetOrderedProducts() 
    {
        var products = await context.Products
            .OrderBy(p => p.Category.Name)
            .ThenByDescending(p => p.Price)
            .ThenBy(p => p.Name)
            .Select(p => new
            {
                ProductName = p.Name,
                p.Price,
                CategoryName = p.Category.Name
            })
            .ToListAsync();

        return Ok(products);
    }


    [HttpGet("DynamicSortedProducts")]
    public async Task<ActionResult> GetDynamicSortedProducts([FromQuery] string[] sortedItems)
    {
        string expression = string.Join(", ", sortedItems);
        var products = await context.Products
            .OrderBy(expression)
            .Select(p => new
            {
                ProductName = p.Name,
                p.Price,
                CategoryName = p.Category.Name
            })
            .ToListAsync();
        return Ok(products);
    }


    [HttpGet("GetPagedProducts")]
    public async Task<ActionResult<PaginationDto<ProductSummaryDto>>> GetPagedProducts([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = context.Products.AsQueryable();
        var totalCount = await query.CountAsync();
        var product = await query
            .OrderBy(p => p.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p=> new ProductSummaryDto
            { 
              Name  = p.Name,
               Price = p.Price, 
               CategoryName = p.Category.Name
            })
            .ToListAsync();
        
        var response = new PaginationDto<ProductSummaryDto>(product , totalCount, pageNumber,pageSize);
        return Ok(response);
    }
    
    [HttpPatch("SimulateConflict/{id}")]
    public async Task<ActionResult<object>> SimulateConflict(int id)
    {
        var options = HttpContext.RequestServices
            .GetRequiredService<DbContextOptions<EcommerceContext>>();
        
        using var contextA = new EcommerceContext(options);
        var productA = await contextA.Products.FindAsync(id);

        using var contextB = new EcommerceContext(options);
        var productB = await contextB.Products.FindAsync(id);

        if (productA != null)
        {
            productA.Stockquantity = 100; 
            await contextA.SaveChangesAsync();
        }

        if (productB != null)
        {
            productB.Stockquantity = 80;
            try
            {
                await contextB.SaveChangesAsync();
                return new { Message = "Success" };
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entry = ex.Entries.Single();
                var databaseValues = await entry.GetDatabaseValuesAsync();
                var dbProduct = (Product)databaseValues.ToObject();

                return new 
                { 
                    Error = "Conflict Detected!", 
                    CurrentDbStock = dbProduct.Stockquantity,
                    YourProposedStock = productB.Stockquantity
                };
            }
        }

        return "Product not found";
    }
    
    
} 

// public static class QueryableExtensions
// { public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName, bool isDescending)
//     {
//         // 1. إنشاء الـ Parameter (الـ p في p => p.Name)
//         var parameter = Expression.Parameter(typeof(T), "p");
//         // 2. الوصول للخاصية (الـ p.Name)
//         // بنستخدم اسم العمود "propertyName" اللي جاي من اليوزر كـ نص
//         var property = Expression.Property(parameter, propertyName);
//         // 3. بناء الـ Lambda Expression (p => p.Name)
//         var lambda = Expression.Lambda(property, parameter);
//         // 4. تحديد اسم الدالة (OrderBy أو OrderByDescending)
//         string methodName = isDescending ? "OrderByDescending" : "OrderBy";
//         // 5. استدعاء الدالة برمجياً
//         var resultExpression = Expression.Call(
//             typeof(Queryable), 
//             methodName, 
//             new Type[] { typeof(T), property.Type },
//             query.Expression, 
//             Expression.Quote(lambda)
//         );
//         return query.Provider.CreateQuery<T>(resultExpression);
//     }
