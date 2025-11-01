using System.ComponentModel.DataAnnotations;

namespace OPME.StockManagement.WebAPI.ViewModels;

public class ProductViewModel
{
    public Guid Id { get; set; }
    public string CodigoProduto { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid SupplierId { get; set; }
    public Guid BrandId { get; set; }
    public string SupplierNome { get; set; } = string.Empty;
    public string BrandNome { get; set; } = string.Empty;
    public int? QuantidadeEstoque { get; set; }
}

public class CreateProductViewModel
{
    [Required(ErrorMessage = "O código do produto é obrigatório")]
    [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
    [Display(Name = "Código do Produto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    [Display(Name = "Nome do Produto")]
    public string NomeProduto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione um fornecedor")]
    [Display(Name = "Fornecedor")]
    public Guid SupplierId { get; set; }

    [Required(ErrorMessage = "Selecione uma marca")]
    [Display(Name = "Marca")]
    public Guid BrandId { get; set; }
}

public class EditProductViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O código do produto é obrigatório")]
    [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres")]
    [Display(Name = "Código do Produto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do produto é obrigatório")]
    [StringLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
    [Display(Name = "Nome do Produto")]
    public string NomeProduto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione um fornecedor")]
    [Display(Name = "Fornecedor")]
    public Guid SupplierId { get; set; }

    [Required(ErrorMessage = "Selecione uma marca")]
    [Display(Name = "Marca")]
    public Guid BrandId { get; set; }
}

public class ProductListViewModel
{
    public IEnumerable<ProductViewModel> Products { get; set; } = new List<ProductViewModel>();
    public string? SearchTerm { get; set; }
    public bool? ShowOnlyActive { get; set; }
}

