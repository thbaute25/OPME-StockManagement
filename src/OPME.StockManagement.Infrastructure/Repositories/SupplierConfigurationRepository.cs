using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Infrastructure.Data;

namespace OPME.StockManagement.Infrastructure.Repositories;

public class SupplierConfigurationRepository : Repository<SupplierConfiguration>, ISupplierConfigurationRepository
{
    public SupplierConfigurationRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<SupplierConfiguration> GetConfigsWithSupplier() => _dbSet.Include(c => c.Supplier);

    public async Task<SupplierConfiguration?> GetBySupplierIdAsync(Guid supplierId)
    {
        return await GetConfigsWithSupplier()
            .FirstOrDefaultAsync(c => c.SupplierId == supplierId);
    }

    public async Task<IEnumerable<SupplierConfiguration>> GetActiveConfigurationsAsync()
    {
        return await GetConfigsWithSupplier()
            .Where(c => c.Ativo)
            .ToListAsync();
    }

    public async Task<IEnumerable<SupplierConfiguration>> GetConfigurationsByDeliveryTimeAsync(int maxDeliveryDays)
    {
        return await GetConfigsWithSupplier()
            .Where(c => c.PrazoEntregaDias <= maxDeliveryDays)
            .ToListAsync();
    }

    public async Task<bool> ExistsBySupplierIdAsync(Guid supplierId)
    {
        return await _dbSet.AnyAsync(c => c.SupplierId == supplierId);
    }
}
