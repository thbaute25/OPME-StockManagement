using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Infrastructure.Data;

namespace OPME.StockManagement.Infrastructure.Repositories;

public class StockOutputRepository : Repository<StockOutput>, IStockOutputRepository
{
    public StockOutputRepository(ApplicationDbContext context) : base(context)
    {
    }

    private IQueryable<StockOutput> GetOutputsWithProduct() => _dbSet.Include(o => o.Product);

    public async Task<IEnumerable<StockOutput>> GetByProductIdAsync(Guid productId)
    {
        return await GetOutputsWithProduct()
            .Where(o => o.ProductId == productId)
            .OrderByDescending(o => o.DataSaida)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockOutput>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await GetOutputsWithProduct()
            .Where(o => o.DataSaida >= startDate && o.DataSaida <= endDate)
            .OrderByDescending(o => o.DataSaida)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockOutput>> GetByProductAndDateRangeAsync(Guid productId, DateTime startDate, DateTime endDate)
    {
        return await GetOutputsWithProduct()
            .Where(o => o.ProductId == productId && o.DataSaida >= startDate && o.DataSaida <= endDate)
            .OrderByDescending(o => o.DataSaida)
            .ToListAsync();
    }

    public async Task<IEnumerable<StockOutput>> GetRecentOutputsAsync(int days)
    {
        var startDate = DateTime.UtcNow.AddDays(-days);
        return await GetOutputsWithProduct()
            .Where(o => o.DataSaida >= startDate)
            .OrderByDescending(o => o.DataSaida)
            .ToListAsync();
    }

    public async Task<int> GetTotalOutputByProductAsync(Guid productId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(o => o.ProductId == productId && o.DataSaida >= startDate && o.DataSaida <= endDate)
            .SumAsync(o => o.Quantidade);
    }

    public async Task<IEnumerable<StockOutput>> GetAllWithProductAsync()
    {
        return await GetOutputsWithProduct()
            .OrderByDescending(o => o.DataSaida)
            .ToListAsync();
    }
}
