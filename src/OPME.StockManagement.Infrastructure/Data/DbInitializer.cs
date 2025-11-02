using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Infrastructure.Data;

public static class DbInitializer
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Products.Any())
            return;

        var suppliers = new List<Supplier>
        {
            new Supplier("MedSupply Brasil", "12345678000190", "(11) 3456-7890", "contato@medsupply.com.br"),
            new Supplier("Equipamentos Hospitalares SA", "98765432000110", "(21) 3234-5678", "vendas@equiphospital.com.br"),
            new Supplier("Medicamentos Farmacêuticos", "11223344000155", "(11) 3233-4455", "comercial@medfarm.com.br"),
            new Supplier("Insumos Médicos Ltda", "55667788000122", "(47) 3344-5566", "contato@insumosmed.com.br"),
            new Supplier("Distribuidora de OPME", "99887766000133", "(85) 3456-7890", "contato@distribopme.com.br"),
            new Supplier("Material Hospitalar Premium", "44556677000144", "(11) 3123-4567", "vendas@materialpremium.com.br")
        };

        var brands = new List<Brand>
        {
            new Brand("MedTech"),
            new Brand("HealthCare"),
            new Brand("BioSafe"),
            new Brand("MedPro")
        };

        foreach (var s in suppliers)
        {
            context.Suppliers.Add(s);
        }

        foreach (var b in brands)
        {
            context.Brands.Add(b);
        }

        context.SaveChanges();

        // Criar configuração para o primeiro fornecedor
        var config = new SupplierConfiguration(
            suppliers[0].Id,
            mesesPlanejamento: 6,
            mesesMinimos: 2,
            prazoEntregaDias: 15);

        context.SupplierConfigurations.Add(config);

        // Criar 5 produtos, cada um com um fornecedor diferente
        var products = new List<Product>
        {
            new Product("PROD001", "Seringa 10ml", suppliers[0].Id, brands[0].Id),
            new Product("PROD002", "Cateter Urinário 14Fr", suppliers[1].Id, brands[1].Id),
            new Product("PROD003", "Máscara Cirúrgica N95", suppliers[2].Id, brands[2].Id),
            new Product("PROD004", "Agulha Descartável 25x7", suppliers[3].Id, brands[3].Id),
            new Product("PROD005", "Luva Cirúrgica Estéril", suppliers[4].Id, brands[0].Id)
        };

        foreach (var product in products)
        {
            context.Products.Add(product);
        }

        context.SaveChanges(); // Salvar produtos primeiro para gerar IDs

        // Criar estoques para cada produto (após salvar para ter os IDs)
        var stocks = new List<CurrentStock>
        {
            new CurrentStock(products[0].Id, 50),
            new CurrentStock(products[1].Id, 30),
            new CurrentStock(products[2].Id, 100),
            new CurrentStock(products[3].Id, 75),
            new CurrentStock(products[4].Id, 120)
        };

        foreach (var stock in stocks)
        {
            context.CurrentStocks.Add(stock);
        }

        context.SaveChanges();
    }
}

