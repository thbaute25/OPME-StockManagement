namespace OPME.StockManagement.Domain.Entities;

public class Supplier
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = null!;
    public string Cnpj { get; private set; } = null!;
    public string Telefone { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation properties
    public ICollection<Product> Products { get; private set; } = new List<Product>();
    public ICollection<SupplierConfiguration> Configurations { get; private set; } = new List<SupplierConfiguration>();

    private Supplier() { } // EF Core

    public Supplier(string nome, string cnpj, string telefone, string email)
    {
        Id = Guid.NewGuid();
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        Telefone = telefone ?? throw new ArgumentNullException(nameof(telefone));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Ativo = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateInfo(string nome, string telefone, string email)
    {
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome obrigatório");
        if (string.IsNullOrWhiteSpace(telefone)) throw new ArgumentException("Telefone obrigatório");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email obrigatório");

        Nome = nome;
        Telefone = telefone;
        Email = email;
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
