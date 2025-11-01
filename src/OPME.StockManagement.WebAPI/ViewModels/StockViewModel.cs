using System.ComponentModel.DataAnnotations;

namespace OPME.StockManagement.WebAPI.ViewModels;

public class StockViewModel
{
    public Guid Id { get; set; }
    public int QuantidadeAtual { get; set; }
    public DateTime DataUltimaAtualizacao { get; set; }
    public Guid ProductId { get; set; }
    public string ProductNome { get; set; } = string.Empty;
    public string ProductCodigo { get; set; } = string.Empty;
}

public class UpdateStockViewModel
{
    [Required]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(0, 10000, ErrorMessage = "A quantidade deve estar entre 0 e 10000")]
    [Display(Name = "Quantidade")]
    public int Quantidade { get; set; }
}

public class AddStockViewModel
{
    [Required]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "A quantidade é obrigatória")]
    [Range(1, 10000, ErrorMessage = "A quantidade deve estar entre 1 e 10000")]
    [Display(Name = "Quantidade a Adicionar")]
    public int Quantidade { get; set; }
}

public class StockListViewModel
{
    public IEnumerable<StockViewModel> Stocks { get; set; } = new List<StockViewModel>();
    public int? MinQuantityFilter { get; set; }
    public bool ShowLowStock { get; set; }
}

