using System;
using System.Collections.Generic;

namespace MyEcommerce.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string Country { get; set; } = null!;

    public string? City { get; set; }

    public int? CustomerId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
