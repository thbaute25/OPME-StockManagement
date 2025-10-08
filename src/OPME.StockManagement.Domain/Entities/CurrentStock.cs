namespace OPME.StockManagement.Domain.Entities;

public class CurrentStock
{
    public Guid Id { get; private set; }
    public int QuantidadeAtual { get; private set; }
    public DateTime DataUltimaAtualizacao { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Foreign Key
    public Guid ProductId { get; private set; }

    // Navigation property
    public Product Product { get; private set; } = null!;

    private CurrentStock() { } // EF Core

    public CurrentStock(Guid productId, int quantidadeInicial = 0)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        QuantidadeAtual = quantidadeInicial;
        DataUltimaAtualizacao = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero");
        QuantidadeAtual += quantidade;
        DataUltimaAtualizacao = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ReduceStock(int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero");
        if (QuantidadeAtual < quantidade) throw new InvalidOperationException("Estoque insuficiente");

        QuantidadeAtual -= quantidade;
        DataUltimaAtualizacao = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStock(int novaQuantidade)
    {
        if (novaQuantidade < 0) throw new ArgumentException("Quantidade não pode ser negativa");
        QuantidadeAtual = novaQuantidade;
        DataUltimaAtualizacao = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsStockLow(int quantidadeMinima) => QuantidadeAtual <= quantidadeMinima;
    public bool IsOutOfStock() => QuantidadeAtual <= 0;
}
