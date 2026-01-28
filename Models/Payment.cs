using System;
using System.Collections.Generic;

namespace MyEcommerce.Models;

public abstract partial class Payment
{
    public int Id { get; set; }

    public decimal Amount { get; set; }

    public int Orderid { get; set; }
    
}

public class CreditCardPayment : Payment
{
    public string? Cardnumber { get; set; }

    public string? Cardholdername { get; set; }
}

public class PayPalPayment : Payment
{
    public string? Email { get; set; }

    public string? Transactionid { get; set; }
}

