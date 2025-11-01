using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IBrandRepository _brandRepository;
    private readonly ICurrentStockRepository _stockRepository;

    public ProductService(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IBrandRepository brandRepository,
        ICurrentStockRepository stockRepository)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _brandRepository = brandRepository;
        _stockRepository = stockRepository;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdWithIncludesAsync(id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllWithIncludesAsync();
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, CreateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new EntityNotFoundException("Produto", id);

        product.UpdateInfo(dto.CodigoProduto, dto.NomeProduto);
        await _productRepository.UpdateAsync(product);
        
        // Buscar com includes para mapear corretamente
        var productWithIncludes = await _productRepository.GetByIdWithIncludesAsync(id);
        return MapToDto(productWithIncludes!);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new EntityNotFoundException("Produto", id);

        await _productRepository.DeleteAsync(product);
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new EntityNotFoundException("Produto", id);

        if (product.Ativo)
            product.Deactivate();
        else
            product.Activate();
            
        await _productRepository.UpdateAsync(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        if (await _productRepository.ExistsByCodigoProdutoAsync(dto.CodigoProduto))
            throw new EntityAlreadyExistsException("Produto", "Código", dto.CodigoProduto);

        var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
        if (supplier == null)
            throw new EntityNotFoundException("Fornecedor", dto.SupplierId);

        var brand = await _brandRepository.GetByIdAsync(dto.BrandId);
        if (brand == null)
            throw new EntityNotFoundException("Marca", dto.BrandId);

        var product = new Product(dto.CodigoProduto, dto.NomeProduto, dto.SupplierId, dto.BrandId);
        var created = await _productRepository.AddAsync(product);

        // Criar estoque inicial
        var stock = new CurrentStock(created.Id, 0);
        await _stockRepository.AddAsync(stock);

        // Buscar com includes para mapear corretamente
        var productWithIncludes = await _productRepository.GetByIdWithIncludesAsync(created.Id);
        return MapToDto(productWithIncludes!);
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            CodigoProduto = product.CodigoProduto,
            NomeProduto = product.NomeProduto,
            Ativo = product.Ativo,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            SupplierId = product.SupplierId,
            BrandId = product.BrandId,
            SupplierNome = product.Supplier?.Nome ?? string.Empty,
            BrandNome = product.Brand?.Nome ?? string.Empty,
            QuantidadeEstoque = product.CurrentStock?.QuantidadeAtual
        };
    }
}
