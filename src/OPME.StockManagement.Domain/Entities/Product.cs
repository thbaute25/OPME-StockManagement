namespace OPME.StockManagement.Domain.Entities;

public class Product
{
    public Guid Id { get; private set; }
    public string CodigoProduto { get; private set; }
    public string NomeProduto { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Foreign Keys
    public Guid SupplierId { get; private set; }
    public Guid BrandId { get; private set; }

    // Navigation properties
    public Supplier Supplier { get; private set; } = null!;
    public Brand Brand { get; private set; } = null!;
    public CurrentStock? CurrentStock { get; private set; }
    public ICollection<StockOutput> StockOutputs { get; private set; } = new List<StockOutput>();

    private Product() { } // EF Core

    public Product(string codigoProduto, string nomeProduto, Guid supplierId, Guid brandId)
    {
        Id = Guid.NewGuid();
        CodigoProduto = codigoProduto ?? throw new ArgumentNullException(nameof(codigoProduto));
        NomeProduto = nomeProduto ?? throw new ArgumentNullException(nameof(nomeProduto));
        SupplierId = supplierId;
        BrandId = brandId;
        Ativo = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string codigoProduto, string nomeProduto)
    {
        if (string.IsNullOrWhiteSpace(codigoProduto)) throw new ArgumentException("Código obrigatório");
        if (string.IsNullOrWhiteSpace(nomeProduto)) throw new ArgumentException("Nome obrigatório");

        CodigoProduto = codigoProduto;
        NomeProduto = nomeProduto;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeSupplier(Guid supplierId)
    {
        if (supplierId == Guid.Empty) throw new ArgumentException("Fornecedor obrigatório");
        SupplierId = supplierId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeBrand(Guid brandId)
    {
        if (brandId == Guid.Empty) throw new ArgumentException("Marca obrigatória");
        BrandId = brandId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        Ativo = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Ativo = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStockOutput(StockOutput stockOutput)
    {
        if (stockOutput == null) throw new ArgumentNullException(nameof(stockOutput));
        if (!Ativo) throw new InvalidOperationException("Produto inativo");
        if (CurrentStock == null) throw new InvalidOperationException("Sem estoque");
        if (CurrentStock.QuantidadeAtual < stockOutput.Quantidade) 
            throw new InvalidOperationException("Estoque insuficiente");

        StockOutputs.Add(stockOutput);
        CurrentStock.ReduceStock(stockOutput.Quantidade);
    }
}
