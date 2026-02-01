using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyEcommerce.DTOs;
using MyEcommerce.Models; 

namespace MyEcommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomersController(EcommerceContext context) : ControllerBase
{


    [HttpGet("summaries")]
    public async Task<ActionResult<IEnumerable<CustomerOrderSummaryDto>>> GetCustomerSummaries()
    {
        var summaries = await context.Customers
            .OrderBy(c => c.Name) 
            .Select(c => new CustomerOrderSummaryDto
            {
                CustomerName = c.Name,
                TotalOrders = c.Orders.Count,
                TotalSpent = c.Orders
                    .SelectMany(o => o.OrderItems)
                    .Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .Take(5)
            .ToListAsync();

        return Ok(summaries);
    }

    [HttpGet("CustomerByOrderStatus")]
    public async Task<ActionResult<IEnumerable<CustomerByOrderStatusDto>>> GetCustomerByOrderStatus()
    {
        var customer = await context.Customers
            .Where(o => o.Orders.Any(o =>o.Status == "completed"))
            .Select(c => new CustomerByOrderStatusDto
            {
                CustomerName = c.Name,
                TotalOrders = c.Orders.Count(o => o.Status == "completed")
            })
            .ToListAsync();
        return customer;
    }


    // [HttpGet("OrdersAndItemsPerCustomer")]
    // public async Task<ActionResult<IEnumerable<CustomerOrderSummaryDto>>> GetOrdersAndItemsPerCustomer()
    // {
    //     var summery = await context.Orders
    //     
    // }
}