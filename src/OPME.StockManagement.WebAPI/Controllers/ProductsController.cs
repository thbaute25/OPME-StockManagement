using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Application.Services;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _productService;
    private readonly IBrandRepository _brandRepository;

    public ProductsController(ProductService productService, IBrandRepository brandRepository)
    {
        _productService = productService;
        _brandRepository = brandRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetActive()
    {
        var products = await _productService.GetActiveAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        try
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            
            return Ok(product);
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        try
        {
            var product = await _productService.CreateAsync(dto);
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
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(Guid id, CreateProductDto dto)
    {
        try
        {
            var product = await _productService.UpdateAsync(id, dto);
            return Ok(product);
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
            await _productService.DeleteAsync(id);
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
            await _productService.ToggleStatusAsync(id);
            return NoContent();
        }
        catch (EntityNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("create-brand")]
    public async Task<ActionResult<BrandDto>> CreateBrand(string nome)
    {
        try
        {
            var brand = new Brand(nome);
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
            return BadRequest(ex.Message);
        }
    }
}

