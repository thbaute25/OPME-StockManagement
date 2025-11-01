using Microsoft.Extensions.Logging;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class SupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(ISupplierRepository supplierRepository, ILogger<SupplierService> logger)
    {
        _supplierRepository = supplierRepository;
        _logger = logger;
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
    {
        if (await _supplierRepository.ExistsByCnpjAsync(dto.Cnpj))
        {
            _logger.LogWarning("CNPJ duplicado: {Cnpj}", dto.Cnpj);
            throw new EntityAlreadyExistsException("Fornecedor", "CNPJ", dto.Cnpj);
        }

        if (await _supplierRepository.ExistsByEmailAsync(dto.Email))
        {
            _logger.LogWarning("Email duplicado: {Email}", dto.Email);
            throw new EntityAlreadyExistsException("Fornecedor", "Email", dto.Email);
        }

        try
        {
            var supplier = new Supplier(dto.Nome, dto.Cnpj, dto.Telefone, dto.Email);
            var created = await _supplierRepository.AddAsync(supplier);
            _logger.LogInformation("Fornecedor criado: {SupplierId} - {Nome}", created.Id, created.Nome);
            return MapToDto(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar fornecedor: {Cnpj}", dto.Cnpj);
            throw;
        }
    }

    public async Task<SupplierDto?> GetByIdAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            _logger.LogWarning("Fornecedor não encontrado: {SupplierId}", id);
            return null;
        }
        return MapToDto(supplier);
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        try
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return suppliers.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar fornecedores");
            throw;
        }
    }

    public async Task<SupplierDto> UpdateAsync(Guid id, CreateSupplierDto dto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            _logger.LogWarning("Fornecedor não encontrado: {SupplierId}", id);
            throw new EntityNotFoundException("Fornecedor", id);
        }

        try
        {
            supplier.UpdateInfo(dto.Nome, dto.Telefone, dto.Email);
            await _supplierRepository.UpdateAsync(supplier);
            _logger.LogInformation("Fornecedor atualizado: {SupplierId}", supplier.Id);
            return MapToDto(supplier);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar fornecedor: {SupplierId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            _logger.LogWarning("Fornecedor não encontrado: {SupplierId}", id);
            throw new EntityNotFoundException("Fornecedor", id);
        }

        try
        {
            await _supplierRepository.DeleteAsync(supplier);
            _logger.LogInformation("Fornecedor excluído: {SupplierId}", supplier.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir fornecedor: {SupplierId}", id);
            throw;
        }
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            _logger.LogWarning("Fornecedor não encontrado: {SupplierId}", id);
            throw new EntityNotFoundException("Fornecedor", id);
        }

        try
        {
            if (supplier.Ativo)
                supplier.Deactivate();
            else
                supplier.Activate();
                
            await _supplierRepository.UpdateAsync(supplier);
            _logger.LogInformation("Status alterado: {SupplierId} - {Status}", supplier.Id, supplier.Ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status: {SupplierId}", id);
            throw;
        }
    }

    private static SupplierDto MapToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            Nome = supplier.Nome,
            Cnpj = supplier.Cnpj,
            Telefone = supplier.Telefone,
            Email = supplier.Email,
            Ativo = supplier.Ativo,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };
    }
}
