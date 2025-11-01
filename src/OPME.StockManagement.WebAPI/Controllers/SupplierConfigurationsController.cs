using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SupplierConfigurationsController : ControllerBase
{
    private readonly SupplierConfigurationService _configService;

    public SupplierConfigurationsController(SupplierConfigurationService configService)
    {
        _configService = configService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierConfigurationDto>>> GetAll()
    {
        var configs = await _configService.GetAllAsync();
        return Ok(configs);
    }

    [HttpGet("supplier/{supplierId}")]
    public async Task<ActionResult<SupplierConfigurationDto>> GetBySupplierId(Guid supplierId)
    {
        var config = await _configService.GetBySupplierIdAsync(supplierId);
        if (config == null)
            return NotFound();
        
        return Ok(config);
    }

    [HttpPost]
    public async Task<ActionResult<SupplierConfigurationDto>> Create(CreateSupplierConfigurationDto dto)
    {
        try
        {
            var config = await _configService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetBySupplierId), new { supplierId = config.SupplierId }, config);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (EntityAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<SupplierConfigurationDto>> Update(Guid id, CreateSupplierConfigurationDto dto)
    {
        try
        {
            var config = await _configService.UpdateAsync(id, dto);
            return Ok(config);
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

    [HttpPatch("{id}/toggle-status")]
    public async Task<ActionResult> ToggleStatus(Guid id)
    {
        try
        {
            await _configService.ToggleStatusAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

