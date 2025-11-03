using Microsoft.Extensions.Logging;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class BrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly ILogger<BrandService> _logger;

    public BrandService(IBrandRepository brandRepository, ILogger<BrandService> logger)
    {
        _brandRepository = brandRepository;
        _logger = logger;
    }

    public async Task<BrandDto> CreateAsync(CreateBrandDto dto)
    {
        var existing = await _brandRepository.GetAllAsync();
        if (existing.Any(b => b.Nome.Equals(dto.Nome, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Marca já existe: {Nome}", dto.Nome);
            throw new EntityAlreadyExistsException("Marca", "nome", dto.Nome);
        }

        var brand = new Brand(dto.Nome);
        await _brandRepository.AddAsync(brand);
        
        _logger.LogInformation("Marca criada: {Id} - {Nome}", brand.Id, brand.Nome);
        return MapToDto(brand);
    }

    public async Task<IEnumerable<BrandDto>> GetAllAsync()
    {
        var brands = await _brandRepository.GetAllAsync();
        return brands.Select(MapToDto);
    }

    public async Task<BrandDto> GetByIdAsync(Guid id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            _logger.LogWarning("Marca não encontrada: {Id}", id);
            throw new EntityNotFoundException("Marca", id);
        }
        return MapToDto(brand);
    }

    public async Task<BrandDto> UpdateAsync(Guid id, CreateBrandDto dto)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            _logger.LogWarning("Marca não encontrada: {Id}", id);
            throw new EntityNotFoundException("Marca", id);
        }

        var existing = await _brandRepository.GetAllAsync();
        if (existing.Any(b => b.Id != id && b.Nome.Equals(dto.Nome, StringComparison.OrdinalIgnoreCase)))
        {
            _logger.LogWarning("Marca já existe: {Nome}", dto.Nome);
            throw new EntityAlreadyExistsException("Marca", "nome", dto.Nome);
        }

        brand.UpdateName(dto.Nome);
        await _brandRepository.UpdateAsync(brand);
        
        _logger.LogInformation("Marca atualizada: {Id} - {Nome}", id, dto.Nome);
        return MapToDto(brand);
    }

    public async Task DeleteAsync(Guid id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            _logger.LogWarning("Marca não encontrada: {Id}", id);
            throw new EntityNotFoundException("Marca", id);
        }

        await _brandRepository.DeleteAsync(brand);
        _logger.LogInformation("Marca excluída: {Id}", id);
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            _logger.LogWarning("Marca não encontrada: {Id}", id);
            throw new EntityNotFoundException("Marca", id);
        }

        if (brand.Ativo)
            brand.Deactivate();
        else
            brand.Activate();
        await _brandRepository.UpdateAsync(brand);
        
        _logger.LogInformation("Status da marca alterado: {Id} - {Ativo}", id, brand.Ativo);
    }

    private static BrandDto MapToDto(Brand brand)
    {
        return new BrandDto
        {
            Id = brand.Id,
            Nome = brand.Nome,
            Ativo = brand.Ativo,
            CreatedAt = brand.CreatedAt,
            UpdatedAt = brand.UpdatedAt
        };
    }
}

