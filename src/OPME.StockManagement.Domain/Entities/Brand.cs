namespace OPME.StockManagement.Domain.Entities;

public class Brand
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Brand() { } // EF Core

    public Brand(string nome)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Ativo = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateName(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome obrigatório");
        Nome = nome;
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
}
