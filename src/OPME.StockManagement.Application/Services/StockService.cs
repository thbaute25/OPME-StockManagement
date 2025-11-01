using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class StockService
{
    private readonly ICurrentStockRepository _stockRepository;
    private readonly IProductRepository _productRepository;

    public StockService(ICurrentStockRepository stockRepository, IProductRepository productRepository)
    {
        _stockRepository = stockRepository;
        _productRepository = productRepository;
    }

    public async Task<CurrentStockDto> GetByProductIdAsync(Guid productId)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
            throw new EntityNotFoundException("Estoque", productId);

        return MapToDto(stock);
    }

    public async Task<IEnumerable<CurrentStockDto>> GetAllAsync()
    {
        var stocks = await _stockRepository.GetAllWithProductAsync();
        return stocks.Select(s => MapToDto(s));
    }

    public async Task<IEnumerable<CurrentStockDto>> GetLowStockAsync(int quantidadeMinima = 10)
    {
        var stocks = await _stockRepository.GetLowStockItemsAsync(quantidadeMinima);
        return stocks.Select(s => MapToDto(s));
    }

    public async Task<CurrentStockDto> AddStockAsync(Guid productId, int quantidade)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
            throw new EntityNotFoundException("Estoque", productId);

        stock.AddStock(quantidade);
        await _stockRepository.UpdateAsync(stock);

        // Buscar novamente com includes
        var stockWithProduct = await _stockRepository.GetByProductIdAsync(productId);
        return MapToDto(stockWithProduct!);
    }

    public async Task<CurrentStockDto> ReduceStockAsync(Guid productId, int quantidade)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
            throw new EntityNotFoundException("Estoque", productId);

        stock.ReduceStock(quantidade);
        await _stockRepository.UpdateAsync(stock);

        // Buscar novamente com includes
        var stockWithProduct = await _stockRepository.GetByProductIdAsync(productId);
        return MapToDto(stockWithProduct!);
    }

    public async Task<CurrentStockDto> SetStockAsync(Guid productId, int quantidade)
    {
        var stock = await _stockRepository.GetByProductIdAsync(productId);
        if (stock == null)
            throw new EntityNotFoundException("Estoque", productId);

        stock.SetStock(quantidade);
        await _stockRepository.UpdateAsync(stock);

        // Buscar novamente com includes
        var stockWithProduct = await _stockRepository.GetByProductIdAsync(productId);
        return MapToDto(stockWithProduct!);
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
