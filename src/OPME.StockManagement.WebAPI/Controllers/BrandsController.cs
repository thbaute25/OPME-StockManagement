using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BrandsController : ControllerBase
{
    private readonly BrandService _brandService;
    private readonly HateoasService _hateoasService;

    public BrandsController(BrandService brandService, HateoasService hateoasService)
    {
        _brandService = brandService;
        _hateoasService = hateoasService;
    }

    /// <summary>
    /// Obtém todas as marcas
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BrandDto>>> GetAll()
    {
        var brands = await _brandService.GetAllAsync();
        foreach (var brand in brands)
        {
            brand.Links = _hateoasService.GetBrandLinks(brand.Id);
        }
        return Ok(brands);
    }

    /// <summary>
    /// Obtém uma marca por ID
    /// </summary>
    /// <param name="id">ID da marca</param>
    /// <returns>Marca encontrada</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BrandDto>> GetById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        try
        {
            var brand = await _brandService.GetByIdAsync(id);
            brand.Links = _hateoasService.GetBrandLinks(id);
            return Ok(brand);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova marca
    /// </summary>
    /// <param name="dto">Dados da marca a ser criada</param>
    /// <returns>Marca criada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BrandDto>> Create([FromBody] CreateBrandDto dto)
    {
        if (dto == null)
            return BadRequest("Dados da marca não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var brand = await _brandService.CreateAsync(dto);
            brand.Links = _hateoasService.GetBrandLinks(brand.Id);
            return CreatedAtAction(nameof(GetById), new { id = brand.Id }, brand);
        }
        catch (EntityAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar marca: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza uma marca existente
    /// </summary>
    /// <param name="id">ID da marca</param>
    /// <param name="dto">Dados atualizados da marca</param>
    /// <returns>Marca atualizada</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BrandDto>> Update([FromRoute] Guid id, [FromBody] CreateBrandDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        if (dto == null)
            return BadRequest("Dados da marca não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var brand = await _brandService.UpdateAsync(id, dto);
            brand.Links = _hateoasService.GetBrandLinks(id);
            return Ok(brand);
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
            return BadRequest($"Erro ao atualizar marca: {ex.Message}");
        }
    }

    /// <summary>
    /// Exclui uma marca
    /// </summary>
    /// <param name="id">ID da marca</param>
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
            await _brandService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao excluir marca: {ex.Message}");
        }
    }

    /// <summary>
    /// Alterna o status ativo/inativo de uma marca
    /// </summary>
    /// <param name="id">ID da marca</param>
    [HttpPatch("{id:guid}/toggle-status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ToggleStatus([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        try
        {
            await _brandService.ToggleStatusAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao alterar status: {ex.Message}");
        }
    }
}

