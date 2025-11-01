using Microsoft.Extensions.Logging;
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
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IBrandRepository brandRepository,
        ICurrentStockRepository stockRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _brandRepository = brandRepository;
        _stockRepository = stockRepository;
        _logger = logger;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdWithIncludesAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Produto não encontrado: {ProductId}", id);
            return null;
        }
        return MapToDto(product);
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        try
        {
            var products = await _productRepository.GetAllWithIncludesAsync();
            return products.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos");
            throw;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetActiveAsync()
    {
        try
        {
            var products = await _productRepository.GetActiveProductsAsync();
            return products.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos ativos");
            throw;
        }
    }

    public async Task<ProductDto> UpdateAsync(Guid id, CreateProductDto dto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Produto não encontrado: {ProductId}", id);
            throw new EntityNotFoundException("Produto", id);
        }

        try
        {
            product.UpdateInfo(dto.CodigoProduto, dto.NomeProduto);
            await _productRepository.UpdateAsync(product);
            
            var productWithIncludes = await _productRepository.GetByIdWithIncludesAsync(id);
            _logger.LogInformation("Produto atualizado: {ProductId}", productWithIncludes!.Id);
            
            return MapToDto(productWithIncludes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto: {ProductId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Produto não encontrado: {ProductId}", id);
            throw new EntityNotFoundException("Produto", id);
        }

        try
        {
            await _productRepository.DeleteAsync(product);
            _logger.LogInformation("Produto excluído: {ProductId}", product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir produto: {ProductId}", id);
            throw;
        }
    }

    public async Task ToggleStatusAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            _logger.LogWarning("Produto não encontrado: {ProductId}", id);
            throw new EntityNotFoundException("Produto", id);
        }

        try
        {
            if (product.Ativo)
                product.Deactivate();
            else
                product.Activate();
                
            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("Status alterado: {ProductId} - {Status}", product.Id, product.Ativo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar status: {ProductId}", id);
            throw;
        }
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        if (await _productRepository.ExistsByCodigoProdutoAsync(dto.CodigoProduto))
        {
            _logger.LogWarning("Código duplicado: {CodigoProduto}", dto.CodigoProduto);
            throw new EntityAlreadyExistsException("Produto", "Código", dto.CodigoProduto);
        }

        var supplier = await _supplierRepository.GetByIdAsync(dto.SupplierId);
        if (supplier == null)
        {
            _logger.LogWarning("Fornecedor não encontrado: {SupplierId}", dto.SupplierId);
            throw new EntityNotFoundException("Fornecedor", dto.SupplierId);
        }

        var brand = await _brandRepository.GetByIdAsync(dto.BrandId);
        if (brand == null)
        {
            _logger.LogWarning("Marca não encontrada: {BrandId}", dto.BrandId);
            throw new EntityNotFoundException("Marca", dto.BrandId);
        }

        try
        {
            var product = new Product(dto.CodigoProduto, dto.NomeProduto, dto.SupplierId, dto.BrandId);
            var created = await _productRepository.AddAsync(product);

            var stock = new CurrentStock(created.Id, 0);
            await _stockRepository.AddAsync(stock);

            var productWithIncludes = await _productRepository.GetByIdWithIncludesAsync(created.Id);
            _logger.LogInformation("Produto criado: {ProductId} - {CodigoProduto}", productWithIncludes!.Id, productWithIncludes.CodigoProduto);
            
            return MapToDto(productWithIncludes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto: {CodigoProduto}", dto.CodigoProduto);
            throw;
        }
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
