namespace MyEcommerce.Models;
using Microsoft.AspNetCore.Identity;
public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? Address { get; set; }
}