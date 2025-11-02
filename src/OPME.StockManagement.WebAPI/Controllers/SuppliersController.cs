using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly SupplierService _supplierService;
    private readonly HateoasService _hateoasService;

    public SuppliersController(SupplierService supplierService, HateoasService hateoasService)
    {
        _supplierService = supplierService;
        _hateoasService = hateoasService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
    {
        var suppliers = await _supplierService.GetAllAsync();
        foreach (var supplier in suppliers)
        {
            supplier.Links = _hateoasService.GetSupplierLinks(supplier.Id);
        }
        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierDto>> GetById(Guid id)
    {
        try
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();
            
            supplier.Links = _hateoasService.GetSupplierLinks(id);
            return Ok(supplier);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierDto dto)
    {
        try
        {
            var supplier = await _supplierService.CreateAsync(dto);
            supplier.Links = _hateoasService.GetSupplierLinks(supplier.Id);
            return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
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
    public async Task<ActionResult<SupplierDto>> Update(Guid id, CreateSupplierDto dto)
    {
        try
        {
            var supplier = await _supplierService.UpdateAsync(id, dto);
            supplier.Links = _hateoasService.GetSupplierLinks(id);
            return Ok(supplier);
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            await _supplierService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPatch("{id}/toggle-status")]
    public async Task<ActionResult> ToggleStatus(Guid id)
    {
        try
        {
            await _supplierService.ToggleStatusAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}

