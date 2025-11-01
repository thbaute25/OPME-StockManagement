using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.Infrastructure.Data;
using OPME.StockManagement.Infrastructure.Repositories;

namespace OPME.StockManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICurrentStockRepository, CurrentStockRepository>();
        services.AddScoped<IStockOutputRepository, StockOutputRepository>();
        services.AddScoped<ISupplierConfigurationRepository, SupplierConfigurationRepository>();

        return services;
    }
}
