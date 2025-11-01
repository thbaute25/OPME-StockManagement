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
