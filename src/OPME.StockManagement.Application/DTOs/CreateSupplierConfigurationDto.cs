namespace OPME.StockManagement.Application.DTOs;

public class CreateSupplierConfigurationDto
{
    public Guid SupplierId { get; set; }
    public int MesesPlanejamento { get; set; }
    public int MesesMinimos { get; set; }
    public int PrazoEntregaDias { get; set; }
}

