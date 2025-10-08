using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Domain.Interfaces;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<Supplier?> GetByCnpjAsync(string cnpj);
    Task<Supplier?> GetByEmailAsync(string email);
    Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
    Task<IEnumerable<Supplier>> GetSuppliersByNameAsync(string nome);
    Task<bool> ExistsByCnpjAsync(string cnpj);
    Task<bool> ExistsByEmailAsync(string email);
}
