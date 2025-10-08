using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Infrastructure.Data;

namespace OPME.StockManagement.Infrastructure.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Supplier?> GetByCnpjAsync(string cnpj)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Cnpj == cnpj);
    }

    public async Task<Supplier?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
    {
        return await _dbSet.Where(s => s.Ativo).ToListAsync();
    }

    public async Task<IEnumerable<Supplier>> GetSuppliersByNameAsync(string nome)
    {
        return await _dbSet.Where(s => s.Nome.Contains(nome)).ToListAsync();
    }

    public async Task<bool> ExistsByCnpjAsync(string cnpj)
    {
        return await _dbSet.AnyAsync(s => s.Cnpj == cnpj);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _dbSet.AnyAsync(s => s.Email == email);
    }
}
