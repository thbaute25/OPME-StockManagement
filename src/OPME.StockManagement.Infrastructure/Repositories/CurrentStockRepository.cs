using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Infrastructure.Data;

namespace OPME.StockManagement.Infrastructure.Repositories;

public class CurrentStockRepository : Repository<CurrentStock>, ICurrentStockRepository
{
    public CurrentStockRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<CurrentStock> GetStockWithProduct() => _dbSet.Include(s => s.Product);

    public async Task<IEnumerable<CurrentStock>> GetAllWithProductAsync()
    {
        return await GetStockWithProduct()
            .ToListAsync();
    }

    public async Task<CurrentStock?> GetByProductIdAsync(Guid productId)
    {
        return await GetStockWithProduct()
            .FirstOrDefaultAsync(s => s.ProductId == productId);
    }

    public async Task<IEnumerable<CurrentStock>> GetLowStockItemsAsync(int quantidadeMinima)
    {
        return await GetStockWithProduct()
            .Where(s => s.QuantidadeAtual <= quantidadeMinima)
            .ToListAsync();
    }

    public async Task<IEnumerable<CurrentStock>> GetOutOfStockItemsAsync()
    {
        return await GetStockWithProduct()
            .Where(s => s.QuantidadeAtual <= 0)
            .ToListAsync();
    }

    public async Task<IEnumerable<CurrentStock>> GetStockItemsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await GetStockWithProduct()
            .Where(s => s.DataUltimaAtualizacao >= startDate && s.DataUltimaAtualizacao <= endDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalStockValueAsync()
    {
        return await _dbSet.SumAsync(s => s.QuantidadeAtual);
    }
}
