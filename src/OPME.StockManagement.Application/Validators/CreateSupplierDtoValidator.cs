using FluentValidation;
using OPME.StockManagement.Application.DTOs;

namespace OPME.StockManagement.Application.Validators;

public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do fornecedor é obrigatório.")
            .MaximumLength(200).WithMessage("O nome não pode exceder 200 caracteres.")
            .MinimumLength(3).WithMessage("O nome deve ter no mínimo 3 caracteres.");

        RuleFor(x => x.Cnpj)
            .NotEmpty().WithMessage("O CNPJ é obrigatório.")
            .Length(14, 18).WithMessage("O CNPJ deve ter entre 14 e 18 caracteres.")
            .Must(BeValidCnpjFormat).WithMessage("CNPJ inválido. Use o formato: XX.XXX.XXX/XXXX-XX ou apenas números.");

        RuleFor(x => x.Telefone)
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .MaximumLength(20).WithMessage("O telefone não pode exceder 20 caracteres.")
            .Matches(@"^[\d\s\(\)\-\+]+$").WithMessage("Formato de telefone inválido.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório.")
            .MaximumLength(100).WithMessage("O email não pode exceder 100 caracteres.")
            .EmailAddress().WithMessage("Email inválido.");
    }

    private bool BeValidCnpjFormat(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        // Remove caracteres não numéricos
        var digitsOnly = System.Text.RegularExpressions.Regex.Replace(cnpj, @"[^\d]", "");
        
        // Deve ter exatamente 14 dígitos
        return digitsOnly.Length == 14;
    }
}

