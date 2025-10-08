using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Domain.Interfaces;

public interface ISupplierConfigurationRepository : IRepository<SupplierConfiguration>
{
    Task<SupplierConfiguration?> GetBySupplierIdAsync(Guid supplierId);
    Task<IEnumerable<SupplierConfiguration>> GetActiveConfigurationsAsync();
    Task<IEnumerable<SupplierConfiguration>> GetConfigurationsByDeliveryTimeAsync(int maxDeliveryDays);
    Task<bool> ExistsBySupplierIdAsync(Guid supplierId);
}
