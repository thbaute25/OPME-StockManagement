using System.ComponentModel.DataAnnotations;

namespace OPME.StockManagement.WebAPI.ViewModels;

public class BrandViewModel
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateBrandViewModel
{
    [Required(ErrorMessage = "O nome da marca é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    [Display(Name = "Nome da Marca")]
    public string Nome { get; set; } = string.Empty;
}

public class EditBrandViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "O nome da marca é obrigatório")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
    [Display(Name = "Nome da Marca")]
    public string Nome { get; set; } = string.Empty;
}

public class BrandListViewModel
{
    public IEnumerable<BrandViewModel> Brands { get; set; } = new List<BrandViewModel>();
    public string? SearchTerm { get; set; }
    public bool? ShowOnlyActive { get; set; }
}

