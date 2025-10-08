namespace OPME.StockManagement.Domain.Entities;

public class StockOutput
{
    public Guid Id { get; private set; }
    public int Quantidade { get; private set; }
    public DateTime DataSaida { get; private set; }
    public string? Observacoes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Foreign Key
    public Guid ProductId { get; private set; }

    // Navigation property
    public Product Product { get; private set; } = null!;

    private StockOutput() { } // EF Core

    public StockOutput(Guid productId, int quantidade, string? observacoes = null)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        Quantidade = quantidade;
        DataSaida = DateTime.UtcNow;
        Observacoes = observacoes;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateQuantity(int novaQuantidade)
    {
        if (novaQuantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero");
        Quantidade = novaQuantidade;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateObservations(string? observacoes)
    {
        Observacoes = observacoes;
        UpdatedAt = DateTime.UtcNow;
    }
}
