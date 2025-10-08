using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CurrentStockDto>>> GetAll()
    {
        var stocks = await _stockService.GetAllAsync();
        return Ok(stocks);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<CurrentStockDto>>> GetLowStock([FromQuery] int minQuantity = 10)
    {
        var stocks = await _stockService.GetLowStockAsync(minQuantity);
        return Ok(stocks);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<CurrentStockDto>> GetByProductId(Guid productId)
    {
        try
        {
            var stock = await _stockService.GetByProductIdAsync(productId);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("product/{productId}/add")]
    public async Task<ActionResult<CurrentStockDto>> AddStock(Guid productId, [FromBody] UpdateStockDto dto)
    {
        try
        {
            var stock = await _stockService.AddStockAsync(productId, dto.Quantidade);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("product/{productId}/reduce")]
    public async Task<ActionResult<CurrentStockDto>> ReduceStock(Guid productId, [FromBody] UpdateStockDto dto)
    {
        try
        {
            var stock = await _stockService.ReduceStockAsync(productId, dto.Quantidade);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("product/{productId}")]
    public async Task<ActionResult<CurrentStockDto>> SetStock(Guid productId, [FromBody] UpdateStockDto dto)
    {
        try
        {
            var stock = await _stockService.SetStockAsync(productId, dto.Quantidade);
            return Ok(stock);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

