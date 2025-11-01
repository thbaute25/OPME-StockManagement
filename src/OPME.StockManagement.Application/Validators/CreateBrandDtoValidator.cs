using FluentValidation;
using OPME.StockManagement.Application.DTOs;

namespace OPME.StockManagement.Application.Validators;

public class CreateBrandDtoValidator : AbstractValidator<CreateBrandDto>
{
    public CreateBrandDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome da marca é obrigatório.")
            .MaximumLength(100).WithMessage("O nome da marca não pode exceder 100 caracteres.")
            .MinimumLength(2).WithMessage("O nome da marca deve ter no mínimo 2 caracteres.");
    }
}

