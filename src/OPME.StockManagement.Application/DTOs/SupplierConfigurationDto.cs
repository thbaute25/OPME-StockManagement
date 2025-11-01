namespace OPME.StockManagement.Application.DTOs;

public class SupplierConfigurationDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string? SupplierNome { get; set; }
    public int MesesPlanejamento { get; set; }
    public int MesesMinimos { get; set; }
    public int PrazoEntregaDias { get; set; }
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

