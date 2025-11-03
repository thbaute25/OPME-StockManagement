using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IBrandRepository _brandRepository;
    private readonly HateoasService _hateoasService;

    public ProductsController(
        ProductService productService, 
        IBrandRepository brandRepository,
        HateoasService hateoasService)
    {
        _productService = productService;
        _brandRepository = brandRepository;
        _hateoasService = hateoasService;
    }

    /// <summary>
    /// Obtém todos os produtos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        foreach (var product in products)
        {
            product.Links = _hateoasService.GetProductLinks(product.Id);
        }
        return Ok(products);
    }

    /// <summary>
    /// Obtém apenas produtos ativos
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetActive()
    {
        var products = await _productService.GetActiveAsync();
        foreach (var product in products)
        {
            product.Links = _hateoasService.GetProductLinks(product.Id);
        }
        return Ok(products);
    }

    /// <summary>
    /// Obtém um produto por ID
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <returns>Produto encontrado</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById([FromRoute] Guid id)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound($"Produto com ID {id} não encontrado");
            
            product.Links = _hateoasService.GetProductLinks(id);
            return Ok(product);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Cria um novo produto
    /// </summary>
    /// <param name="dto">Dados do produto a ser criado</param>
    /// <returns>Produto criado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        if (dto == null)
            return BadRequest("Dados do produto não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var product = await _productService.CreateAsync(dto);
            product.Links = _hateoasService.GetProductLinks(product.Id);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (EntityAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar produto: {ex.Message}");
        }
    }

    /// <summary>
    /// Atualiza um produto existente
    /// </summary>
    /// <param name="id">ID do produto</param>
    /// <param name="dto">Dados atualizados do produto</param>
    /// <returns>Produto atualizado</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Update([FromRoute] Guid id, [FromBody] CreateProductDto dto)
    {
        if (id == Guid.Empty)
            return BadRequest("ID inválido");

        if (dto == null)
            return BadRequest("Dados do produto não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var product = await _productService.UpdateAsync(id, dto);
            product.Links = _hateoasService.GetProductLinks(id);
            return Ok(product);
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
            return BadRequest($"Erro ao atualizar produto: {ex.Message}");
        }
    }

    /// <summary>
    /// Exclui um produto
    /// </summary>
    /// <param name="id">ID do produto</param>
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
            await _productService.DeleteAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao excluir produto: {ex.Message}");
        }
    }

    /// <summary>
    /// Alterna o status ativo/inativo de um produto
    /// </summary>
    /// <param name="id">ID do produto</param>
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
            await _productService.ToggleStatusAsync(id);
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
    /// Cria uma nova marca (endpoint legado - usar /api/brands)
    /// </summary>
    /// <param name="dto">Dados da marca</param>
    /// <returns>Marca criada</returns>
    [HttpPost("create-brand")]
    [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Obsolete("Use POST /api/brands instead")]
    public async Task<ActionResult<BrandDto>> CreateBrand([FromBody] CreateBrandDto dto)
    {
        if (dto == null)
            return BadRequest("Dados da marca não podem ser nulos");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var brand = new Brand(dto.Nome);
            await _brandRepository.AddAsync(brand);
            
            return Ok(new BrandDto
            {
                Id = brand.Id,
                Nome = brand.Nome,
                Ativo = brand.Ativo,
                CreatedAt = brand.CreatedAt,
                UpdatedAt = brand.UpdatedAt
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Erro ao criar marca: {ex.Message}");
        }
    }
}

