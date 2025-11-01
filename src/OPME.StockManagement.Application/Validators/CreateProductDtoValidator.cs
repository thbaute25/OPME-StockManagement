using FluentValidation;
using OPME.StockManagement.Application.DTOs;

namespace OPME.StockManagement.Application.Validators;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.CodigoProduto)
            .NotEmpty().WithMessage("O código do produto é obrigatório.")
            .MaximumLength(50).WithMessage("O código do produto não pode exceder 50 caracteres.")
            .MinimumLength(2).WithMessage("O código do produto deve ter no mínimo 2 caracteres.")
            .Matches(@"^[A-Za-z0-9\-\_]+$").WithMessage("O código do produto pode conter apenas letras, números, hífen e underscore.");

        RuleFor(x => x.NomeProduto)
            .NotEmpty().WithMessage("O nome do produto é obrigatório.")
            .MaximumLength(200).WithMessage("O nome do produto não pode exceder 200 caracteres.")
            .MinimumLength(3).WithMessage("O nome do produto deve ter no mínimo 3 caracteres.");

        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("O ID do fornecedor é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O ID do fornecedor é inválido.");

        RuleFor(x => x.BrandId)
            .NotEmpty().WithMessage("O ID da marca é obrigatório.")
            .NotEqual(Guid.Empty).WithMessage("O ID da marca é inválido.");
    }
}

