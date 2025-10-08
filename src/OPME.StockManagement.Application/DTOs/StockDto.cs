namespace OPME.StockManagement.Application.DTOs;

public class CurrentStockDto
{
    public Guid Id { get; set; }
    public int QuantidadeAtual { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
    public Guid ProductId { get; set; }
    public string ProductNome { get; set; } = string.Empty;
    public string ProductCodigo { get; set; } = string.Empty;
}

public class StockOutputDto
{
    public Guid Id { get; set; }
    public int Quantidade { get; set; }
    public DateTime DataSaida { get; set; }
    public string? Observacoes { get; set; }
    public Guid ProductId { get; set; }
    public string ProductNome { get; set; } = string.Empty;
    public string ProductCodigo { get; set; } = string.Empty;
}

public class CreateStockOutputDto
{
    public Guid ProductId { get; set; }
    public int Quantidade { get; set; }
    public string? Observacoes { get; set; }
}

public class UpdateStockDto
{
    public int Quantidade { get; set; }
}
