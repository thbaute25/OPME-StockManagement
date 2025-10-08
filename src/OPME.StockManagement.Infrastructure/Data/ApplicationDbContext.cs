using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Domain.Entities;

namespace OPME.StockManagement.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<CurrentStock> CurrentStocks { get; set; }
    public DbSet<StockOutput> StockOutputs { get; set; }
    public DbSet<SupplierConfiguration> SupplierConfigurations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Supplier Configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(14);
            entity.Property(e => e.Telefone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.Cnpj).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasMany(e => e.Products)
                  .WithOne(p => p.Supplier)
                  .HasForeignKey(p => p.SupplierId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Brand Configuration
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasMany(e => e.Products)
                  .WithOne(p => p.Brand)
                  .HasForeignKey(p => p.BrandId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Product Configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CodigoProduto).IsRequired().HasMaxLength(50);
            entity.Property(e => e.NomeProduto).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.CodigoProduto).IsUnique();

            entity.HasOne(e => e.CurrentStock)
                  .WithOne(s => s.Product)
                  .HasForeignKey<CurrentStock>(s => s.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.StockOutputs)
                  .WithOne(o => o.Product)
                  .HasForeignKey(o => o.ProductId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // CurrentStock Configuration
        modelBuilder.Entity<CurrentStock>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.QuantidadeAtual).IsRequired();
            entity.Property(e => e.DataUltimaAtualizacao).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.ProductId).IsUnique();
        });

        // StockOutput Configuration
        modelBuilder.Entity<StockOutput>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantidade).IsRequired();
            entity.Property(e => e.DataSaida).IsRequired();
            entity.Property(e => e.Observacoes).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.DataSaida);
        });

        // SupplierConfiguration Configuration
        modelBuilder.Entity<SupplierConfiguration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MesesPlanejamento).IsRequired();
            entity.Property(e => e.MesesMinimos).IsRequired();
            entity.Property(e => e.PrazoEntregaDias).IsRequired();
            entity.Property(e => e.Ativo).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            entity.HasIndex(e => e.SupplierId).IsUnique();

            entity.HasOne(e => e.Supplier)
                  .WithMany(s => s.Configurations)
                  .HasForeignKey(e => e.SupplierId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
