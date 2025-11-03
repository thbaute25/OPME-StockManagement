using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;
    private readonly HateoasService _hateoasService;

    public StockController(StockService stockService, HateoasService hateoasService)
    {
        _stockService = stockService;
        _hateoasService = hateoasService;
    }

    /// <summary>
    /// Obtém todo o estoque
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CurrentStockDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CurrentStockDto>>> GetAll()
    {
        var stocks = await _stockService.GetAllAsync();
        foreach (var stock in stocks)
        {
            stock.Links = _hateoasService.GetStockLinks(stock.Id, stock.ProductId);
        }
        return Ok(stocks);
    }

    /// <summary>
    /// Obtém produtos com estoque baixo
    /// </summary>
    /// <param name="minQuantity">Quantidade mínima (padrão: 10)</param>
    /// <returns>Lista de estoques com quantidade abaixo do mínimo</returns>
    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IEnumerable<CurrentStockDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<CurrentStockDto>>> GetLowStock([FromQuery] int minQuantity = 10)
    {
        if (minQuantity < 0)
            return BadRequest("Quantidade mínima deve ser maior ou igual a zero");

        var stocks = await _stockService.GetLowStockAsync(minQuantity);
        foreach (var stock in stocks)
        {
            stock.Links = _hateoasService.GetStockLinks(stock.Id, stock.ProductId);
        }
        return Ok(stocks);
    }

    /// <summary>
    /// Obtém o estoque de um produto específico
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <returns>Estoque do produto</returns>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(CurrentStockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentStockDto>> GetByProductId([FromRoute] Guid productId)
    {
        if (productId == Guid.Empty)
            return BadRequest("ID do produto inválido");

        try
        {
            var stock = await _stockService.GetByProductIdAsync(productId);
            stock.Links = _hateoasService.GetStockLinks(stock.Id, productId);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Adiciona quantidade ao estoque de um produto
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <param name="dto">Quantidade a adicionar</param>
    /// <returns>Estoque atualizado</returns>
    [HttpPost("product/{productId:guid}/add")]
    [ProducesResponseType(typeof(CurrentStockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentStockDto>> AddStock([FromRoute] Guid productId, [FromBody] UpdateStockDto dto)
    {
        if (productId == Guid.Empty)
            return BadRequest("ID do produto inválido");

        if (dto == null)
            return BadRequest("Dados não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var stock = await _stockService.AddStockAsync(productId, dto.Quantidade);
            stock.Links = _hateoasService.GetStockLinks(stock.Id, productId);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao adicionar estoque: {ex.Message}");
        }
    }

    /// <summary>
    /// Reduz quantidade do estoque de um produto
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <param name="dto">Quantidade a reduzir</param>
    /// <returns>Estoque atualizado</returns>
    [HttpPost("product/{productId:guid}/reduce")]
    [ProducesResponseType(typeof(CurrentStockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentStockDto>> ReduceStock([FromRoute] Guid productId, [FromBody] UpdateStockDto dto)
    {
        if (productId == Guid.Empty)
            return BadRequest("ID do produto inválido");

        if (dto == null)
            return BadRequest("Dados não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var stock = await _stockService.ReduceStockAsync(productId, dto.Quantidade);
            stock.Links = _hateoasService.GetStockLinks(stock.Id, productId);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao reduzir estoque: {ex.Message}");
        }
    }

    /// <summary>
    /// Define a quantidade exata do estoque de um produto
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <param name="dto">Nova quantidade do estoque</param>
    /// <returns>Estoque atualizado</returns>
    [HttpPut("product/{productId:guid}")]
    [ProducesResponseType(typeof(CurrentStockDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentStockDto>> SetStock([FromRoute] Guid productId, [FromBody] UpdateStockDto dto)
    {
        if (productId == Guid.Empty)
            return BadRequest("ID do produto inválido");

        if (dto == null)
            return BadRequest("Dados não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (dto.Quantidade < 0)
            return BadRequest("Quantidade não pode ser negativa");

        try
        {
            var stock = await _stockService.SetStockAsync(productId, dto.Quantidade);
            stock.Links = _hateoasService.GetStockLinks(stock.Id, productId);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao definir estoque: {ex.Message}");
        }
    }
}

