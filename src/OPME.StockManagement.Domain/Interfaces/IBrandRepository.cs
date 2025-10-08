using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Domain.Interfaces;

public interface IBrandRepository : IRepository<Brand>
{
    Task<Brand?> GetByNameAsync(string nome);
    Task<IEnumerable<Brand>> GetActiveBrandsAsync();
    Task<IEnumerable<Brand>> GetBrandsByNameAsync(string nome);
    Task<bool> ExistsByNameAsync(string nome);
}
