using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SupplierConfigurationsController : ControllerBase
{
    private readonly SupplierConfigurationService _configService;
    private readonly HateoasService _hateoasService;

    public SupplierConfigurationsController(SupplierConfigurationService configService, HateoasService hateoasService)
    {
        _configService = configService;
        _hateoasService = hateoasService;
    }

    /// <summary>
    /// Obtém todas as configurações de fornecedores
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SupplierConfigurationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SupplierConfigurationDto>>> GetAll()
    {
        var configs = await _configService.GetAllAsync();
        foreach (var config in configs)
        {
            config.Links = _hateoasService.GetSupplierConfigurationLinks(config.Id, config.SupplierId);
        }
        return Ok(configs);
    }

    /// <summary>
    /// Obtém a configuração de um fornecedor específico
    /// </summary>
    /// <param name="supplierId">ID do fornecedor</param>
    /// <returns>Configuração do fornecedor</returns>
    [HttpGet("supplier/{supplierId:guid}")]
    [ProducesResponseType(typeof(SupplierConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierConfigurationDto>> GetBySupplierId([FromRoute] Guid supplierId)
    {
        if (supplierId == Guid.Empty)
            return BadRequest("ID do fornecedor inválido");

        var config = await _configService.GetBySupplierIdAsync(supplierId);
        if (config == null)
            return NotFound($"Configuração para o fornecedor {supplierId} não encontrada");
        
        config.Links = _hateoasService.GetSupplierConfigurationLinks(config.Id, supplierId);
        return Ok(config);
    }

    /// <summary>
    /// Obtém uma configuração por ID
    /// </summary>
    /// <param name="id">ID da configuração</param>
    /// <returns>Configuração encontrada</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SupplierConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierConfigurationDto>> GetById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        try
        {
            var config = await _configService.GetByIdAsync(id);
            config.Links = _hateoasService.GetSupplierConfigurationLinks(id, config.SupplierId);
            return Ok(config);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Cria uma nova configuração de fornecedor
    /// </summary>
    /// <param name="dto">Dados da configuração</param>
    /// <returns>Configuração criada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SupplierConfigurationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SupplierConfigurationDto>> Create([FromBody] CreateSupplierConfigurationDto dto)
    {
        if (dto == null)
            return BadRequest("Dados da configuração não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var config = await _configService.CreateAsync(dto);
            config.Links = _hateoasService.GetSupplierConfigurationLinks(config.Id, config.SupplierId);
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
            return BadRequest($"Erro ao criar configuração: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza uma configuração de fornecedor existente
    /// </summary>
    /// <param name="id">ID da configuração</param>
    /// <param name="dto">Dados atualizados da configuração</param>
    /// <returns>Configuração atualizada</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SupplierConfigurationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierConfigurationDto>> Update([FromRoute] Guid id, [FromBody] CreateSupplierConfigurationDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        if (dto == null)
            return BadRequest("Dados da configuração não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var config = await _configService.UpdateAsync(id, dto);
            config.Links = _hateoasService.GetSupplierConfigurationLinks(id, config.SupplierId);
            return Ok(config);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao atualizar configuração: {ex.Message}");
        }
    }

    /// <summary>
    /// Alterna o status ativo/inativo de uma configuração de fornecedor
    /// </summary>
    /// <param name="id">ID da configuração</param>
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
            await _configService.ToggleStatusAsync(id);
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

    /// <summary>
    /// Exclui uma configuração de fornecedor
    /// </summary>
    /// <param name="id">ID da configuração</param>
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
            await _configService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao excluir configuração: {ex.Message}");
        }
    }
}

