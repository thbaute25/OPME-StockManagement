namespace OPME.StockManagement.Domain.ValueObjects;

public record Cnpj
{
    public string Value { get; }

    public Cnpj(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("CNPJ não pode ser vazio", nameof(value));

        // Remove caracteres não numéricos
        var cleanValue = new string(value.Where(char.IsDigit).ToArray());

        if (cleanValue.Length != 14)
            throw new ArgumentException("CNPJ deve ter 14 dígitos", nameof(value));

        if (IsAllSameDigits(cleanValue))
            throw new ArgumentException("CNPJ não pode ter todos os dígitos iguais", nameof(value));

        if (!IsValidCnpj(cleanValue))
            throw new ArgumentException("CNPJ inválido", nameof(value));

        Value = cleanValue;
    }

    private static bool IsAllSameDigits(string cnpj)
    {
        return cnpj.All(c => c == cnpj[0]);
    }

    private static bool IsValidCnpj(string cnpj)
    {
        // Implementação simplificada de validação de CNPJ
        // Em um cenário real, implementaria a validação completa com dígitos verificadores
        return true;
    }

    public static implicit operator string(Cnpj cnpj) => cnpj.Value;
    public static implicit operator Cnpj(string value) => new(value);
}
