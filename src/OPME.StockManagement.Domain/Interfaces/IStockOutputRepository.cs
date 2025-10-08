using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Domain.Interfaces;

public interface IStockOutputRepository : IRepository<StockOutput>
{
    Task<IEnumerable<StockOutput>> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<StockOutput>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<StockOutput>> GetByProductAndDateRangeAsync(Guid productId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<StockOutput>> GetRecentOutputsAsync(int days);
    Task<int> GetTotalOutputByProductAsync(Guid productId, DateTime startDate, DateTime endDate);
}
