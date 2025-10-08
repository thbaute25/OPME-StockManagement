namespace OPME.StockManagement.Application.DTOs;

public class BrandDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBrandDto
{
    public string Nome { get; set; } = string.Empty;
}
