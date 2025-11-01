using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Infrastructure.Data;

namespace OPME.StockManagement.Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<Product> GetProductsWithIncludes()
    {
        return _dbSet
            .Include(p => p.Supplier)
            .Include(p => p.Brand)
            .Include(p => p.CurrentStock);
    }

    public async Task<Product?> GetByIdWithIncludesAsync(Guid id)
    {
        return await GetProductsWithIncludes()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllWithIncludesAsync()
    {
        return await GetProductsWithIncludes()
            .ToListAsync();
    }

    public async Task<Product?> GetByCodigoProdutoAsync(string codigoProduto)
    {
        return await GetProductsWithIncludes()
            .FirstOrDefaultAsync(p => p.CodigoProduto == codigoProduto);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await GetProductsWithIncludes()
            .Where(p => p.Ativo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsBySupplierAsync(Guid supplierId)
    {
        return await GetProductsWithIncludes()
            .Where(p => p.SupplierId == supplierId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByBrandAsync(Guid brandId)
    {
        return await GetProductsWithIncludes()
            .Where(p => p.BrandId == brandId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByNameAsync(string nome)
    {
        return await GetProductsWithIncludes()
            .Where(p => p.NomeProduto.Contains(nome))
            .ToListAsync();
    }

    public async Task<bool> ExistsByCodigoProdutoAsync(string codigoProduto)
    {
        return await _dbSet.AnyAsync(p => p.CodigoProduto == codigoProduto);
    }

    public async Task<IEnumerable<Product>> GetProductsWithLowStockAsync(int quantidadeMinima)
    {
        return await GetProductsWithIncludes()
            .Where(p => p.CurrentStock != null && p.CurrentStock.QuantidadeAtual <= quantidadeMinima)
            .ToListAsync();
    }
}
