using FluentValidation;
using OPME.StockManagement.Application.DTOs;

namespace OPME.StockManagement.Application.Validators;

public class UpdateStockDtoValidator : AbstractValidator<UpdateStockDto>
{
    public UpdateStockDtoValidator()
    {
        RuleFor(x => x.Quantidade)
            .GreaterThanOrEqualTo(0).WithMessage("A quantidade não pode ser negativa.")
            .LessThanOrEqualTo(int.MaxValue).WithMessage("A quantidade excede o valor máximo permitido.");
    }
}

