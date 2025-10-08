using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class SupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
    {
        if (await _supplierRepository.ExistsByCnpjAsync(dto.Cnpj))
            throw new EntityAlreadyExistsException("Fornecedor", "CNPJ", dto.Cnpj);

        if (await _supplierRepository.ExistsByEmailAsync(dto.Email))
            throw new EntityAlreadyExistsException("Fornecedor", "Email", dto.Email);

        var supplier = new Supplier(dto.Nome, dto.Cnpj, dto.Telefone, dto.Email);
        var created = await _supplierRepository.AddAsync(supplier);
        
        return MapToDto(created);
    }

    public async Task<SupplierDto?> GetByIdAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        return supplier == null ? null : MapToDto(supplier);
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(MapToDto);
    }

    public async Task<SupplierDto> UpdateAsync(Guid id, CreateSupplierDto dto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            throw new EntityNotFoundException("Fornecedor", id);

        supplier.UpdateInfo(dto.Nome, dto.Telefone, dto.Email);
        await _supplierRepository.UpdateAsync(supplier);
        
        return MapToDto(supplier);
    }

    public async Task DeleteAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            throw new EntityNotFoundException("Fornecedor", id);

        await _supplierRepository.DeleteAsync(supplier);
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            throw new EntityNotFoundException("Fornecedor", id);

        if (supplier.Ativo)
            supplier.Deactivate();
        else
            supplier.Activate();
            
        await _supplierRepository.UpdateAsync(supplier);
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
