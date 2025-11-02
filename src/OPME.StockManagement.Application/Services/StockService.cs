using Microsoft.Extensions.Logging;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class StockService
{
    private readonly ICurrentStockRepository _stockRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<StockService> _logger;

    public StockService(ICurrentStockRepository stockRepository, IProductRepository productRepository, ILogger<StockService> logger)
    {
        _stockRepository = stockRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<CurrentStockDto> GetByProductIdAsync(Guid productId)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
        {
            _logger.LogWarning("Estoque não encontrado: {ProductId}", productId);
            throw new EntityNotFoundException("Estoque", productId);
        }
        return MapToDto(stock);
    }

    public async Task<IEnumerable<CurrentStockDto>> GetAllAsync()
    {
        try
        {
            var stocks = await _stockRepository.GetAllWithProductAsync();
            return stocks.Select(s => MapToDto(s));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estoques");
            throw;
        }
    }

    public async Task<IEnumerable<CurrentStockDto>> GetLowStockAsync(int quantidadeMinima = 10)
    {
        try
        {
            var stocks = await _stockRepository.GetLowStockItemsAsync(quantidadeMinima);
            var count = stocks.Count();
            if (count > 0)
                _logger.LogWarning("Estoque baixo encontrado: {Count} produtos", count);
            return stocks.Select(s => MapToDto(s));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estoque baixo");
            throw;
        }
    }

    public async Task<CurrentStockDto> AddStockAsync(Guid productId, int quantidade)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
        {
            _logger.LogWarning("Estoque não encontrado: {ProductId}", productId);
            throw new EntityNotFoundException("Estoque", productId);
        }

        try
        {
            stock.AddStock(quantidade);
            await _stockRepository.UpdateAsync(stock);
            var stockWithProduct = await _stockRepository.GetByProductIdAsync(productId);
            _logger.LogInformation("Estoque adicionado: {ProductId} - {Quantidade} (total: {Total})", 
                productId, quantidade, stockWithProduct!.QuantidadeAtual);
            return MapToDto(stockWithProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao adicionar estoque: {ProductId}", productId);
            throw;
        }
    }

    public async Task<CurrentStockDto> ReduceStockAsync(Guid productId, int quantidade)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
        {
            _logger.LogWarning("Estoque não encontrado: {ProductId}", productId);
            throw new EntityNotFoundException("Estoque", productId);
        }

        try
        {
            stock.ReduceStock(quantidade);
            await _stockRepository.UpdateAsync(stock);
            var stockWithProduct = await _stockRepository.GetByProductIdAsync(productId);
            _logger.LogInformation("Estoque reduzido: {ProductId} - {Quantidade} (total: {Total})", 
                productId, quantidade, stockWithProduct!.QuantidadeAtual);
            return MapToDto(stockWithProduct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao reduzir estoque: {ProductId}", productId);
            throw;
        }
    }

    public async Task<CurrentStockDto> SetStockAsync(Guid productId, int quantidade)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
        {
            _logger.LogWarning("Estoque não encontrado: {ProductId}", productId);
            throw new EntityNotFoundException("Estoque", productId);
        }

        try
        {
            stock.SetStock(quantidade);
            await _stockRepository.UpdateAsync(stock);
            var stockWithProduct = await _stockRepository.GetByProductIdAsync(productId);
            _logger.LogInformation("Estoque definido: {ProductId} - {Quantidade}", productId, quantidade);
            return MapToDto(stockWithProduct!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao definir estoque: {ProductId}", productId);
            throw;
        }
    }

    public async Task<PagedResult<CurrentStockDto>> SearchAsync(StockSearchParams searchParams)
    {
        try
        {
            var stocks = await _stockRepository.GetAllWithProductAsync();
            
            // Aplicar filtros
            var query = stocks.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var term = searchParams.SearchTerm.ToLowerInvariant();
                query = query.Where(s => 
                    (s.Product != null && s.Product.NomeProduto.ToLower().Contains(term)) ||
                    (s.Product != null && s.Product.CodigoProduto.ToLower().Contains(term)));
            }
            
            if (searchParams.ProductId.HasValue)
            {
                query = query.Where(s => s.ProductId == searchParams.ProductId.Value);
            }
            
            if (searchParams.MinQuantity.HasValue)
            {
                query = query.Where(s => s.QuantidadeAtual >= searchParams.MinQuantity.Value);
            }
            
            if (searchParams.MaxQuantity.HasValue)
            {
                query = query.Where(s => s.QuantidadeAtual <= searchParams.MaxQuantity.Value);
            }
            
            if (searchParams.LowStockOnly == true)
            {
                var minQty = searchParams.MinQuantity ?? 10;
                query = query.Where(s => s.QuantidadeAtual <= minQty);
            }
            
            // Aplicar ordenação
            if (!string.IsNullOrWhiteSpace(searchParams.SortBy))
            {
                var sortBy = searchParams.SortBy.ToLowerInvariant();
                var isDesc = searchParams.SortDirection?.ToLowerInvariant() == "desc";
                
                query = sortBy switch
                {
                    "produto" => isDesc ? query.OrderByDescending(s => s.Product != null ? s.Product.NomeProduto : "") : query.OrderBy(s => s.Product != null ? s.Product.NomeProduto : ""),
                    "quantidade" => isDesc ? query.OrderByDescending(s => s.QuantidadeAtual) : query.OrderBy(s => s.QuantidadeAtual),
                    "atualizado" => isDesc ? query.OrderByDescending(s => s.DataUltimaAtualizacao) : query.OrderBy(s => s.DataUltimaAtualizacao),
                    _ => query.OrderBy(s => s.QuantidadeAtual)
                };
            }
            else
            {
                query = query.OrderBy(s => s.QuantidadeAtual);
            }
            
            // Contar total
            var totalCount = query.Count();
            
            // Aplicar paginação
            var page = Math.Max(1, searchParams.Page);
            var pageSize = Math.Max(1, Math.Min(100, searchParams.PageSize));
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            return new PagedResult<CurrentStockDto>
            {
                Items = items.Select(s => MapToDto(s)),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar estoques");
            throw;
        }
    }

    private static CurrentStockDto MapToDto(CurrentStock stock, Product? product = null)
    {
        // Se product não foi passado, usar a propriedade de navegação já carregada
        product ??= stock.Product;
        
        return new CurrentStockDto
        {
            Id = stock.Id,
            QuantidadeAtual = stock.QuantidadeAtual,
            DataUltimaAtualizacao = stock.DataUltimaAtualizacao,
            ProductId = stock.ProductId,
            ProductNome = product?.NomeProduto ?? string.Empty,
            ProductCodigo = product?.CodigoProduto ?? string.Empty
        };
    }
}
