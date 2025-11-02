namespace OPME.StockManagement.Application.DTOs;

public class ProductSearchParams : SearchQueryParams
{
    public bool? Ativo { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid? BrandId { get; set; }
    public int? MinStockQuantity { get; set; }
    public int? MaxStockQuantity { get; set; }
}

