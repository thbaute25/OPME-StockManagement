using Microsoft.Extensions.Logging;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class SupplierConfigurationService
{
    private readonly ISupplierConfigurationRepository _configRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly ILogger<SupplierConfigurationService> _logger;

    public SupplierConfigurationService(
        ISupplierConfigurationRepository configRepository,
        ISupplierRepository supplierRepository,
        ILogger<SupplierConfigurationService> logger)
    {
        _configRepository = configRepository;
        _supplierRepository = supplierRepository;
        _logger = logger;
    }

    public async Task<SupplierConfigurationDto> CreateAsync(CreateSupplierConfigurationDto dto)
    {
        if (await _supplierRepository.GetByIdAsync(dto.SupplierId) == null)
        {
            _logger.LogWarning("Fornecedor não encontrado: {SupplierId}", dto.SupplierId);
            throw new EntityNotFoundException("Fornecedor", dto.SupplierId);
        }

        if (await _configRepository.ExistsBySupplierIdAsync(dto.SupplierId))
        {
            _logger.LogWarning("Configuração duplicada: {SupplierId}", dto.SupplierId);
            throw new EntityAlreadyExistsException("Configuração", "Fornecedor", dto.SupplierId.ToString());
        }

        try
        {
            var config = new SupplierConfiguration(
                dto.SupplierId,
                dto.MesesPlanejamento,
                dto.MesesMinimos,
                dto.PrazoEntregaDias);

            var created = await _configRepository.AddAsync(config);
            _logger.LogInformation("Configuração criada: {ConfigId}", created.Id);
            return await MapToDtoAsync(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar configuração: {SupplierId}", dto.SupplierId);
            throw;
        }
    }

    public async Task<SupplierConfigurationDto?> GetBySupplierIdAsync(Guid supplierId)
    {
        var config = await _configRepository.GetBySupplierIdAsync(supplierId);
        if (config == null)
            return null;
        return await MapToDtoAsync(config);
    }

    public async Task<IEnumerable<SupplierConfigurationDto>> GetAllAsync()
    {
        try
        {
            var configs = await _configRepository.GetAllAsync();
            var suppliers = await _supplierRepository.GetAllAsync();
            var supplierDict = suppliers.ToDictionary(s => s.Id);
            
            return configs.Select(c => MapToDto(c, supplierDict.GetValueOrDefault(c.SupplierId)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar configurações");
            throw;
        }
    }

    public async Task<SupplierConfigurationDto> UpdateAsync(Guid id, CreateSupplierConfigurationDto dto)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
        {
            _logger.LogWarning("Configuração não encontrada: {ConfigId}", id);
            throw new EntityNotFoundException("Configuração", id);
        }

        try
        {
            config.UpdateConfiguration(dto.MesesPlanejamento, dto.MesesMinimos, dto.PrazoEntregaDias);
            await _configRepository.UpdateAsync(config);
            
            var updated = await _configRepository.GetBySupplierIdAsync(config.SupplierId);
            _logger.LogInformation("Configuração atualizada: {ConfigId}", config.Id);
            return await MapToDtoAsync(updated!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar configuração: {ConfigId}", id);
            throw;
        }
    }

    public async Task<SupplierConfigurationDto> GetByIdAsync(Guid id)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
        {
            _logger.LogWarning("Configuração não encontrada: {ConfigId}", id);
            throw new EntityNotFoundException("Configuração", id);
        }
        return await MapToDtoAsync(config);
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
        {
            _logger.LogWarning("Configuração não encontrada: {ConfigId}", id);
            throw new EntityNotFoundException("Configuração", id);
        }

        try
        {
            if (config.Ativo)
                config.Deactivate();
            else
                config.Activate();
                
            await _configRepository.UpdateAsync(config);
            _logger.LogInformation("Status alterado: {ConfigId} - {Status}", config.Id, config.Ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status: {ConfigId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var config = await _configRepository.GetByIdAsync(id);
        if (config == null)
        {
            _logger.LogWarning("Configuração não encontrada: {ConfigId}", id);
            throw new EntityNotFoundException("Configuração", id);
        }

        try
        {
            await _configRepository.DeleteAsync(config);
            _logger.LogInformation("Configuração excluída: {ConfigId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir configuração: {ConfigId}", id);
            throw;
        }
    }

    private async Task<SupplierConfigurationDto> MapToDtoAsync(SupplierConfiguration config)
    {
        var supplier = await _supplierRepository.GetByIdAsync(config.SupplierId);
        return MapToDto(config, supplier);
    }

    private static SupplierConfigurationDto MapToDto(SupplierConfiguration config, Domain.Entities.Supplier? supplier = null)
    {
        return new SupplierConfigurationDto
        {
            Id = config.Id,
            SupplierId = config.SupplierId,
            SupplierNome = supplier?.Nome,
            MesesPlanejamento = config.MesesPlanejamento,
            MesesMinimos = config.MesesMinimos,
            PrazoEntregaDias = config.PrazoEntregaDias,
            Ativo = config.Ativo,
            CreatedAt = config.CreatedAt,
            UpdatedAt = config.UpdatedAt
        };
    }
}

