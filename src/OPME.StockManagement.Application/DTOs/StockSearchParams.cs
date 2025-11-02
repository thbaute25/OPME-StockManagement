namespace OPME.StockManagement.Application.DTOs;

public class StockSearchParams : SearchQueryParams
{
    public Guid? ProductId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public bool? LowStockOnly { get; set; }
}

