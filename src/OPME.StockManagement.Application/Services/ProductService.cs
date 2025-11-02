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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ISupplierRepository supplierRepository,
        IBrandRepository brandRepository,
        ICurrentStockRepository stockRepository,
        IUnitOfWork unitOfWork,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _supplierRepository = supplierRepository;
        _brandRepository = brandRepository;
        _stockRepository = stockRepository;
        _unitOfWork = unitOfWork;
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
            await _unitOfWork.BeginTransactionAsync();
            
            var product = new Product(dto.CodigoProduto, dto.NomeProduto, dto.SupplierId, dto.BrandId);
            _productRepository.Add(product);
            _stockRepository.Add(new CurrentStock(product.Id, 0));
            
            await _unitOfWork.CommitTransactionAsync();

            var productWithIncludes = await _productRepository.GetByIdWithIncludesAsync(product.Id);
            _logger.LogInformation("Produto criado: {ProductId} - {CodigoProduto}", productWithIncludes!.Id, productWithIncludes.CodigoProduto);
            
            return MapToDto(productWithIncludes);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Erro ao criar produto: {CodigoProduto}", dto.CodigoProduto);
            throw;
        }
    }

    public async Task<PagedResult<ProductDto>> SearchAsync(ProductSearchParams searchParams)
    {
        try
        {
            var products = await _productRepository.GetAllWithIncludesAsync();
            
            // Aplicar filtros
            var query = products.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var term = searchParams.SearchTerm.ToLowerInvariant();
                query = query.Where(p => 
                    p.NomeProduto.ToLower().Contains(term) ||
                    p.CodigoProduto.ToLower().Contains(term) ||
                    (p.Supplier != null && p.Supplier.Nome.ToLower().Contains(term)) ||
                    (p.Brand != null && p.Brand.Nome.ToLower().Contains(term)));
            }
            
            if (searchParams.Ativo.HasValue)
            {
                query = query.Where(p => p.Ativo == searchParams.Ativo.Value);
            }
            
            if (searchParams.SupplierId.HasValue)
            {
                query = query.Where(p => p.SupplierId == searchParams.SupplierId.Value);
            }
            
            if (searchParams.BrandId.HasValue)
            {
                query = query.Where(p => p.BrandId == searchParams.BrandId.Value);
            }
            
            if (searchParams.MinStockQuantity.HasValue)
            {
                query = query.Where(p => p.CurrentStock != null && 
                    p.CurrentStock.QuantidadeAtual >= searchParams.MinStockQuantity.Value);
            }
            
            if (searchParams.MaxStockQuantity.HasValue)
            {
                query = query.Where(p => p.CurrentStock != null && 
                    p.CurrentStock.QuantidadeAtual <= searchParams.MaxStockQuantity.Value);
            }
            
            // Aplicar ordenação
            if (!string.IsNullOrWhiteSpace(searchParams.SortBy))
            {
                var sortBy = searchParams.SortBy.ToLowerInvariant();
                var isDesc = searchParams.SortDirection?.ToLowerInvariant() == "desc";
                
                query = sortBy switch
                {
                    "nome" => isDesc ? query.OrderByDescending(p => p.NomeProduto) : query.OrderBy(p => p.NomeProduto),
                    "codigo" => isDesc ? query.OrderByDescending(p => p.CodigoProduto) : query.OrderBy(p => p.CodigoProduto),
                    "fornecedor" => isDesc ? query.OrderByDescending(p => p.Supplier != null ? p.Supplier.Nome : "") : query.OrderBy(p => p.Supplier != null ? p.Supplier.Nome : ""),
                    "marca" => isDesc ? query.OrderByDescending(p => p.Brand != null ? p.Brand.Nome : "") : query.OrderBy(p => p.Brand != null ? p.Brand.Nome : ""),
                    "estoque" => isDesc ? query.OrderByDescending(p => p.CurrentStock != null ? p.CurrentStock.QuantidadeAtual : 0) : query.OrderBy(p => p.CurrentStock != null ? p.CurrentStock.QuantidadeAtual : 0),
                    "criado" => isDesc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.NomeProduto)
                };
            }
            else
            {
                query = query.OrderBy(p => p.NomeProduto);
            }
            
            // Contar total
            var totalCount = query.Count();
            
            // Aplicar paginação
            var page = Math.Max(1, searchParams.Page);
            var pageSize = Math.Max(1, Math.Min(100, searchParams.PageSize));
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            return new PagedResult<ProductDto>
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos");
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
