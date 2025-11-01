using FluentValidation;
using OPME.StockManagement.Application.DTOs;

namespace OPME.StockManagement.Application.Validators;

public class CreateStockOutputDtoValidator : AbstractValidator<CreateStockOutputDto>
{
    public CreateStockOutputDtoValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("O ID do produto é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O ID do produto é inválido.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.")
            .LessThanOrEqualTo(10000).WithMessage("A quantidade não pode exceder 10000 unidades.");

        RuleFor(x => x.Observacoes)
            .MaximumLength(500).WithMessage("As observações não podem exceder 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Observacoes));
    }
}

