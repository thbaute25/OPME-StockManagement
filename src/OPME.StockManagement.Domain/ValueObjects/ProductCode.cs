namespace OPME.StockManagement.Domain.ValueObjects;

public record ProductCode
{
    public string Value { get; }

    public ProductCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Código do produto não pode ser vazio", nameof(value));

        if (value.Length < 3)
            throw new ArgumentException("Código do produto deve ter pelo menos 3 caracteres", nameof(value));

        if (value.Length > 50)
            throw new ArgumentException("Código do produto não pode ter mais de 50 caracteres", nameof(value));

        // Remove espaços e converte para maiúsculo
        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(ProductCode productCode) => productCode.Value;
    public static implicit operator ProductCode(string value) => new(value);
}
