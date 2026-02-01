using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MyEcommerce.Middlewares;
using MyEcommerce.Models; 
using Scalar.AspNetCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddOpenApi();

builder.Services.AddDbContext<EcommerceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}
app.UseRequestLogging();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();