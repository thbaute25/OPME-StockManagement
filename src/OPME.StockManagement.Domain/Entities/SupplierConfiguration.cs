namespace OPME.StockManagement.Domain.Entities;

public class SupplierConfiguration
{
    public Guid Id { get; private set; }
    public int MesesPlanejamento { get; private set; }
    public int MesesMinimos { get; private set; }
    public int PrazoEntregaDias { get; private set; }
    public bool Ativo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Foreign Key
    public Guid SupplierId { get; private set; }

    // Navigation property
    public Supplier Supplier { get; private set; } = null!;

    private SupplierConfiguration() { } // EF Core

    public SupplierConfiguration(Guid supplierId, int mesesPlanejamento, int mesesMinimos, int prazoEntregaDias)
    {
        Id = Guid.NewGuid();
        SupplierId = supplierId;
        MesesPlanejamento = mesesPlanejamento;
        MesesMinimos = mesesMinimos;
        PrazoEntregaDias = prazoEntregaDias;
        Ativo = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateConfiguration(int mesesPlanejamento, int mesesMinimos, int prazoEntregaDias)
    {
        if (mesesPlanejamento <= 0) throw new ArgumentException("Meses planejamento deve ser maior que zero");
        if (mesesMinimos <= 0) throw new ArgumentException("Meses mínimos deve ser maior que zero");
        if (prazoEntregaDias <= 0) throw new ArgumentException("Prazo entrega deve ser maior que zero");
        if (mesesMinimos > mesesPlanejamento) throw new ArgumentException("Meses mínimos não pode ser maior que planejamento");

        MesesPlanejamento = mesesPlanejamento;
        MesesMinimos = mesesMinimos;
        PrazoEntregaDias = prazoEntregaDias;
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

    public DateTime CalculateDeliveryDate() => DateTime.UtcNow.AddDays(PrazoEntregaDias);
    public int CalculateReorderQuantity(int consumoMedioMensal) => consumoMedioMensal * MesesPlanejamento;
}
