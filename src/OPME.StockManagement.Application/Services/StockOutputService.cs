using Microsoft.Extensions.Logging;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Exceptions;
using OPME.StockManagement.Domain.Entities;
using OPME.StockManagement.Domain.Interfaces;

namespace OPME.StockManagement.Application.Services;

public class StockOutputService
{
    private readonly IStockOutputRepository _outputRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentStockRepository _stockRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StockOutputService> _logger;

    public StockOutputService(
        IStockOutputRepository outputRepository,
        IProductRepository productRepository,
        ICurrentStockRepository stockRepository,
        IUnitOfWork unitOfWork,
        ILogger<StockOutputService> logger)
    {
        _outputRepository = outputRepository;
        _productRepository = productRepository;
        _stockRepository = stockRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<StockOutputDto> CreateAsync(CreateStockOutputDto dto)
    {
        var product = await _productRepository.GetByIdAsync(dto.ProductId);
        if (product == null)
        {
            _logger.LogWarning("Produto não encontrado: {ProductId}", dto.ProductId);
            throw new EntityNotFoundException("Produto", dto.ProductId);
        }

        var stock = await _stockRepository.GetByProductIdAsync(dto.ProductId);
        if (stock == null)
        {
            _logger.LogWarning("Estoque não encontrado: {ProductId}", dto.ProductId);
            throw new EntityNotFoundException("Estoque", dto.ProductId);
        }

        if (stock.QuantidadeAtual < dto.Quantidade)
        {
            _logger.LogWarning("Estoque insuficiente: {ProductId} - disponível: {Disponivel}, solicitado: {Solicitado}", 
                dto.ProductId, stock.QuantidadeAtual, dto.Quantidade);
            throw new InvalidOperationException($"Estoque insuficiente. Disponível: {stock.QuantidadeAtual}, Solicitado: {dto.Quantidade}");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var output = new StockOutput(dto.ProductId, dto.Quantidade, dto.Observacoes);
            _outputRepository.Add(output);
            stock.ReduceStock(dto.Quantidade);
            _stockRepository.Update(stock);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Saída de estoque criada: {OutputId} - {ProductId} - {Quantidade}", 
                output.Id, dto.ProductId, dto.Quantidade);

            var outputWithProduct = await _outputRepository.GetByIdAsync(output.Id);
            return MapToDto(outputWithProduct!);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<StockOutputDto>> GetAllAsync()
    {
        var outputs = await _outputRepository.GetAllWithProductAsync();
        return outputs.Select(MapToDto);
    }

    public async Task<StockOutputDto> GetByIdAsync(Guid id)
    {
        var output = await _outputRepository.GetByIdAsync(id);
        if (output == null)
        {
            _logger.LogWarning("Saída de estoque não encontrada: {Id}", id);
            throw new EntityNotFoundException("Saída de Estoque", id);
        }
        
        var outputsByProduct = await _outputRepository.GetByProductIdAsync(output.ProductId);
        var outputWithProduct = outputsByProduct.FirstOrDefault(o => o.Id == id);
        return MapToDto(outputWithProduct ?? output);
    }

    public async Task<IEnumerable<StockOutputDto>> GetByProductIdAsync(Guid productId)
    {
        var outputs = await _outputRepository.GetByProductIdAsync(productId);
        return outputs.Select(MapToDto);
    }

    public async Task<IEnumerable<StockOutputDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var outputs = await _outputRepository.GetByDateRangeAsync(startDate, endDate);
        return outputs.Select(MapToDto);
    }

    public async Task<IEnumerable<StockOutputDto>> GetRecentOutputsAsync(int days = 30)
    {
        var outputs = await _outputRepository.GetRecentOutputsAsync(days);
        return outputs.Select(MapToDto);
    }

    public async Task DeleteAsync(Guid id)
    {
        var output = await _outputRepository.GetByIdAsync(id);
        if (output == null)
        {
            _logger.LogWarning("Saída de estoque não encontrada: {Id}", id);
            throw new EntityNotFoundException("Saída de Estoque", id);
        }

        var stock = await _stockRepository.GetByProductIdAsync(output.ProductId);
        if (stock == null)
        {
            _logger.LogWarning("Estoque não encontrado: {ProductId}", output.ProductId);
            throw new EntityNotFoundException("Estoque", output.ProductId);
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            stock.AddStock(output.Quantidade);
            _stockRepository.Update(stock);
            _outputRepository.Delete(output);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Saída de estoque excluída e estoque revertido: {OutputId} - {ProductId} - {Quantidade}",
                id, output.ProductId, output.Quantidade);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static StockOutputDto MapToDto(StockOutput output)
    {
        return new StockOutputDto
        {
            Id = output.Id,
            Quantidade = output.Quantidade,
            DataSaida = output.DataSaida,
            Observacoes = output.Observacoes,
            ProductId = output.ProductId,
            ProductNome = output.Product?.NomeProduto ?? string.Empty,
            ProductCodigo = output.Product?.CodigoProduto ?? string.Empty
        };
    }
}

