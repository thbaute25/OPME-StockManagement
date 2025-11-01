using FluentValidation;
using OPME.StockManagement.Application.DTOs;

namespace OPME.StockManagement.Application.Validators;

public class CreateSupplierConfigurationDtoValidator : AbstractValidator<CreateSupplierConfigurationDto>
{
    public CreateSupplierConfigurationDtoValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Fornecedor é obrigatório");

        RuleFor(x => x.MesesPlanejamento)
            .GreaterThan(0).WithMessage("Meses de planejamento deve ser maior que zero")
            .LessThanOrEqualTo(24).WithMessage("Meses de planejamento não pode ser maior que 24");

        RuleFor(x => x.MesesMinimos)
            .GreaterThan(0).WithMessage("Meses mínimos deve ser maior que zero")
            .LessThanOrEqualTo(12).WithMessage("Meses mínimos não pode ser maior que 12");

        RuleFor(x => x.PrazoEntregaDias)
            .GreaterThan(0).WithMessage("Prazo de entrega deve ser maior que zero")
            .LessThanOrEqualTo(365).WithMessage("Prazo de entrega não pode ser maior que 365 dias");

        RuleFor(x => x.MesesMinimos)
            .LessThanOrEqualTo(x => x.MesesPlanejamento)
            .WithMessage("Meses mínimos não pode ser maior que meses de planejamento")
            .When(x => x.MesesPlanejamento > 0);
    }
}

