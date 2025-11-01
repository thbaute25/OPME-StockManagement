using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdWithIncludesAsync(Guid id);
    Task<IEnumerable<Product>> GetAllWithIncludesAsync();
    Task<Product?> GetByCodigoProdutoAsync(string codigoProduto);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> GetProductsBySupplierAsync(Guid supplierId);
    Task<IEnumerable<Product>> GetProductsByBrandAsync(Guid brandId);
    Task<IEnumerable<Product>> GetProductsByNameAsync(string nome);
    Task<bool> ExistsByCodigoProdutoAsync(string codigoProduto);
    Task<IEnumerable<Product>> GetProductsWithLowStockAsync(int quantidadeMinima);
}
