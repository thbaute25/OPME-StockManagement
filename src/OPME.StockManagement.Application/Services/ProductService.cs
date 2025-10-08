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

        return await MapToDtoAsync(created);
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : await MapToDtoAsync(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var result = new List<ProductDto>();
        
        foreach (var product in products)
        {
            result.Add(await MapToDtoAsync(product));
        }
        
        return result;
    }

    public async Task<IEnumerable<ProductDto>> GetActiveAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        var result = new List<ProductDto>();
        
        foreach (var product in products)
        {
            result.Add(await MapToDtoAsync(product));
        }
        
        return result;
    }

    public async Task<ProductDto> UpdateAsync(Guid id, CreateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new EntityNotFoundException("Produto", id);

        product.UpdateInfo(dto.CodigoProduto, dto.NomeProduto);
        await _productRepository.UpdateAsync(product);
        
        return await MapToDtoAsync(product);
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

    private async Task<ProductDto> MapToDtoAsync(Product product)
    {
        var supplier = await _supplierRepository.GetByIdAsync(product.SupplierId);
        var brand = await _brandRepository.GetByIdAsync(product.BrandId);
        var stock = await _stockRepository.GetByProductIdAsync(product.Id);

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
            SupplierNome = supplier?.Nome ?? string.Empty,
            BrandNome = brand?.Nome ?? string.Empty,
            QuantidadeEstoque = stock?.QuantidadeAtual
        };
    }
}
