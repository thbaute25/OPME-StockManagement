using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Domain.Interfaces;

public interface ICurrentStockRepository : IRepository<CurrentStock>
{
    Task<CurrentStock?> GetByProductIdAsync(Guid productId);
    Task<IEnumerable<CurrentStock>> GetLowStockItemsAsync(int quantidadeMinima);
    Task<IEnumerable<CurrentStock>> GetOutOfStockItemsAsync();
    Task<IEnumerable<CurrentStock>> GetStockItemsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalStockValueAsync();
}
