using System.ComponentModel.DataAnnotations;

namespace OPME.StockManagement.WebAPI.ViewModels;

public class StockOutputViewModel
{
    public Guid Id { get; set; }
    public int Quantidade { get; set; }
    public DateTime DataSaida { get; set; }
    public string? Observacoes { get; set; }
    public Guid ProductId { get; set; }
    public string ProductNome { get; set; } = string.Empty;
    public string ProductCodigo { get; set; } = string.Empty;
}

public class CreateStockOutputViewModel
{
    [Required(ErrorMessage = "Selecione um produto")]
    [Display(Name = "Produto")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(1, 10000, ErrorMessage = "A quantidade deve estar entre 1 e 10000")]
    [Display(Name = "Quantidade")]
    public int Quantidade { get; set; }

    [StringLength(500, ErrorMessage = "As observações devem ter no máximo 500 caracteres")]
    [Display(Name = "Observações")]
    public string? Observacoes { get; set; }
}

public class StockOutputListViewModel
{
    public IEnumerable<StockOutputViewModel> StockOutputs { get; set; } = new List<StockOutputViewModel>();
    public Guid? ProductIdFilter { get; set; }
    public DateTime? StartDateFilter { get; set; }
    public DateTime? EndDateFilter { get; set; }
}

