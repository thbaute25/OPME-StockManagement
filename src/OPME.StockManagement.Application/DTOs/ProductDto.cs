namespace OPME.StockManagement.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string CodigoProduto { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid SupplierId { get; set; }
    public Guid BrandId { get; set; }
    public string SupplierNome { get; set; } = string.Empty;
    public string BrandNome { get; set; } = string.Empty;
    public int? QuantidadeEstoque { get; set; }
    public List<Link>? Links { get; set; }
}

public class CreateProductDto
{
    public string CodigoProduto { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public Guid BrandId { get; set; }
}
