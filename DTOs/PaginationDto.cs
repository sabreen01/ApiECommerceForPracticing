namespace MyEcommerce.DTOs;

public class PaginationDto<T>
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public IEnumerable<T>? Data { get; set; }

    public PaginationDto(IEnumerable<T> data, int count, int pageNumber, int pageSize )
    {
        Data = data;
        TotalCount = count;
        CurrentPage = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)count / pageSize);
        
    }
    
}


public class ProductSummaryDto
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string CategoryName { get; set; }
}