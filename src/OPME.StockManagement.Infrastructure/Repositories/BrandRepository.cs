using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Infrastructure.Data;

namespace OPME.StockManagement.Infrastructure.Repositories;

public class BrandRepository : Repository<Brand>, IBrandRepository
{
    public BrandRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Brand?> GetByNameAsync(string nome)
    {
        return await _dbSet.FirstOrDefaultAsync(b => b.Nome == nome);
    }

    public async Task<IEnumerable<Brand>> GetActiveBrandsAsync()
    {
        return await _dbSet.Where(b => b.Ativo).ToListAsync();
    }

    public async Task<IEnumerable<Brand>> GetBrandsByNameAsync(string nome)
    {
        return await _dbSet.Where(b => b.Nome.Contains(nome)).ToListAsync();
    }

    public async Task<bool> ExistsByNameAsync(string nome)
    {
        return await _dbSet.AnyAsync(b => b.Nome == nome);
    }
}
