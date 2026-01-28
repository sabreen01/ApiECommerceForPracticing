using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Scalar.AspNetCore; // مكتبة التوثيق الحديثة
using MyEcommerce.Models; // تأكدي من Namespace مشروعك

var builder = WebApplication.CreateBuilder(args);

// 1. إضافة المتحكمات (Controllers)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    // هذا السطر يمنع الدوران اللانهائي
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// 2. تفعيل OpenAPI (ضروري لـ Scalar)
builder.Services.AddOpenApi();

// 3. ربط DbContext بـ PostgreSQL (نقرأ البيانات من appsettings.json)
builder.Services.AddDbContext<EcommerceContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 4. إعداد واجهة الـ API في بيئة التطوير
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // توليد ملف التوصيف JSON
    app.MapScalarApiReference(); // تشغيل واجهة Scalar الجميلة
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();