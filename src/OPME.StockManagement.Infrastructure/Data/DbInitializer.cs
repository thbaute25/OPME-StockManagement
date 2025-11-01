using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Infrastructure.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Products.Any())
            return;

        var supplier = new Supplier(
            "MedSupply Brasil",
            "12345678000190",
            "(11) 3456-7890",
            "contato@medsupply.com.br");

        var brand = new Brand("MedTech");

        context.Suppliers.Add(supplier);
        context.Brands.Add(brand);
        context.SaveChanges();

        var config = new SupplierConfiguration(
            supplier.Id,
            mesesPlanejamento: 6,
            mesesMinimos: 2,
            prazoEntregaDias: 15);

        var product = new Product(
            "PROD001",
            "Seringa 10ml",
            supplier.Id,
            brand.Id);

        var stock = new CurrentStock(product.Id, 50);

        context.SupplierConfigurations.Add(config);
        context.Products.Add(product);
        context.CurrentStocks.Add(stock);
        context.SaveChanges();
    }
}

