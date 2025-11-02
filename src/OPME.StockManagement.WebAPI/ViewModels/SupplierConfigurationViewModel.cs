using System.ComponentModel.DataAnnotations;

namespace OPME.StockManagement.WebAPI.ViewModels;

public class SupplierConfigurationViewModel
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string? SupplierNome { get; set; }
    public int MesesPlanejamento { get; set; }
    public int MesesMinimos { get; set; }
    public int PrazoEntregaDias { get; set; }
    public bool Ativo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateSupplierConfigurationViewModel
{
    [Required(ErrorMessage = "Selecione um fornecedor")]
    [Display(Name = "Fornecedor")]
    public Guid SupplierId { get; set; }

    [Required(ErrorMessage = "Os meses de planejamento são obrigatórios")]
    [Range(1, 24, ErrorMessage = "Os meses de planejamento devem estar entre 1 e 24")]
    [Display(Name = "Meses de Planejamento")]
    public int MesesPlanejamento { get; set; }

    [Required(ErrorMessage = "Os meses mínimos são obrigatórios")]
    [Range(1, 24, ErrorMessage = "Os meses mínimos devem estar entre 1 e 24")]
    [Display(Name = "Meses Mínimos")]
    public int MesesMinimos { get; set; }

    [Required(ErrorMessage = "O prazo de entrega é obrigatório")]
    [Range(1, 365, ErrorMessage = "O prazo de entrega deve estar entre 1 e 365 dias")]
    [Display(Name = "Prazo de Entrega (dias)")]
    public int PrazoEntregaDias { get; set; }
}

public class EditSupplierConfigurationViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Selecione um fornecedor")]
    [Display(Name = "Fornecedor")]
    public Guid SupplierId { get; set; }

    [Required(ErrorMessage = "Os meses de planejamento são obrigatórios")]
    [Range(1, 24, ErrorMessage = "Os meses de planejamento devem estar entre 1 e 24")]
    [Display(Name = "Meses de Planejamento")]
    public int MesesPlanejamento { get; set; }

    [Required(ErrorMessage = "Os meses mínimos são obrigatórios")]
    [Range(1, 24, ErrorMessage = "Os meses mínimos devem estar entre 1 e 24")]
    [Display(Name = "Meses Mínimos")]
    public int MesesMinimos { get; set; }

    [Required(ErrorMessage = "O prazo de entrega é obrigatório")]
    [Range(1, 365, ErrorMessage = "O prazo de entrega deve estar entre 1 e 365 dias")]
    [Display(Name = "Prazo de Entrega (dias)")]
    public int PrazoEntregaDias { get; set; }
}

public class SupplierConfigurationListViewModel
{
    public IEnumerable<SupplierConfigurationViewModel> Configurations { get; set; } = new List<SupplierConfigurationViewModel>();
    public string? SearchTerm { get; set; }
    public bool? ShowOnlyActive { get; set; }
}

