using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StockOutputsController : ControllerBase
{
    private readonly StockOutputService _stockOutputService;
    private readonly HateoasService _hateoasService;

    public StockOutputsController(StockOutputService stockOutputService, HateoasService hateoasService)
    {
        _stockOutputService = stockOutputService;
        _hateoasService = hateoasService;
    }

    /// <summary>
    /// Obtém todas as saídas de estoque
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<StockOutputDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StockOutputDto>>> GetAll()
    {
        var outputs = await _stockOutputService.GetAllAsync();
        foreach (var output in outputs)
        {
            output.Links = _hateoasService.GetStockOutputLinks(output.Id, output.ProductId);
        }
        return Ok(outputs);
    }

    /// <summary>
    /// Obtém uma saída de estoque por ID
    /// </summary>
    /// <param name="id">ID da saída de estoque</param>
    /// <returns>Saída de estoque encontrada</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(StockOutputDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockOutputDto>> GetById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        try
        {
            var output = await _stockOutputService.GetByIdAsync(id);
            output.Links = _hateoasService.GetStockOutputLinks(output.Id, output.ProductId);
            return Ok(output);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova saída de estoque
    /// </summary>
    /// <param name="dto">Dados da saída de estoque</param>
    /// <returns>Saída de estoque criada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(StockOutputDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockOutputDto>> Create([FromBody] CreateStockOutputDto dto)
    {
        if (dto == null)
            return BadRequest("Dados da saída de estoque não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var output = await _stockOutputService.CreateAsync(dto);
            output.Links = _hateoasService.GetStockOutputLinks(output.Id, output.ProductId);
            return CreatedAtAction(nameof(GetById), new { id = output.Id }, output);
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
            return BadRequest($"Erro ao criar saída de estoque: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtém todas as saídas de estoque de um produto
    /// </summary>
    /// <param name="productId">ID do produto</param>
    /// <returns>Lista de saídas de estoque do produto</returns>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<StockOutputDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<StockOutputDto>>> GetByProductId([FromRoute] Guid productId)
    {
        if (productId == Guid.Empty)
            return BadRequest("ID do produto inválido");

        var outputs = await _stockOutputService.GetByProductIdAsync(productId);
        foreach (var output in outputs)
        {
            output.Links = _hateoasService.GetStockOutputLinks(output.Id, productId);
        }
        return Ok(outputs);
    }

    /// <summary>
    /// Obtém saídas de estoque por período
    /// </summary>
    /// <param name="startDate">Data inicial</param>
    /// <param name="endDate">Data final</param>
    /// <returns>Lista de saídas de estoque no período</returns>
    [HttpGet("date-range")]
    [ProducesResponseType(typeof(IEnumerable<StockOutputDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<StockOutputDto>>> GetByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
            return BadRequest("Data inicial não pode ser maior que a data final");

        if ((endDate - startDate).Days > 365)
            return BadRequest("Período não pode ser maior que 365 dias");

        var outputs = await _stockOutputService.GetByDateRangeAsync(startDate, endDate);
        foreach (var output in outputs)
        {
            output.Links = _hateoasService.GetStockOutputLinks(output.Id, output.ProductId);
        }
        return Ok(outputs);
    }

    /// <summary>
    /// Obtém saídas de estoque recentes (últimos N dias)
    /// </summary>
    /// <param name="days">Número de dias (padrão: 30, máximo: 365)</param>
    /// <returns>Lista de saídas de estoque recentes</returns>
    [HttpGet("recent")]
    [ProducesResponseType(typeof(IEnumerable<StockOutputDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<StockOutputDto>>> GetRecent([FromQuery] int days = 30)
    {
        if (days < 1 || days > 365)
            return BadRequest("Dias deve estar entre 1 e 365");

        var outputs = await _stockOutputService.GetRecentOutputsAsync(days);
        foreach (var output in outputs)
        {
            output.Links = _hateoasService.GetStockOutputLinks(output.Id, output.ProductId);
        }
        return Ok(outputs);
    }

    /// <summary>
    /// Exclui uma saída de estoque e reverte a quantidade ao estoque
    /// </summary>
    /// <param name="id">ID da saída de estoque</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        try
        {
            await _stockOutputService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao excluir saída de estoque: {ex.Message}");
        }
    }
}

