using MyEcommerce.DTOs;

namespace MyEcommerce.Services.Interfaces;

public interface IReportService
{
    Task<IEnumerable<TopProductsPerCategoryDto>> GetTop3ProductPerCategory();
    Task<IEnumerable<SalesByCustomerCityDto>> GetCityAnalysis();
    Task<IEnumerable<MonthlySalesDto>> GetMonthlySalesForSalse();
    Task<object> GetTotalRevenue();
    Task<object> GetSales(int id);
   

}