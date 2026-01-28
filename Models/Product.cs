using System;
using System.Collections.Generic;

namespace MyEcommerce.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public int? CategoryId { get; set; }

    public int? Stockquantity { get; set; }
    public uint Version { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
