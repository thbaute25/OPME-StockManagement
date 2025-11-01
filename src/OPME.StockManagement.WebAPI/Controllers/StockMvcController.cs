using Microsoft.AspNetCore.Mvc;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Services;
using OPME.StockManagement.WebAPI.ViewModels;

namespace OPME.StockManagement.WebAPI.Controllers;

public class StockMvcController : Controller
{
    private readonly StockService _stockService;

    public StockMvcController(StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? minQuantity)
    {
        IEnumerable<CurrentStockDto> stocks;

        if (minQuantity.HasValue)
        {
            stocks = await _stockService.GetLowStockAsync(minQuantity.Value);
        }
        else
        {
            stocks = await _stockService.GetAllAsync();
        }

        var viewModel = new StockListViewModel
        {
            Stocks = stocks.Select(s => new StockViewModel
            {
                Id = s.Id,
                QuantidadeAtual = s.QuantidadeAtual,
                DataUltimaAtualizacao = s.DataUltimaAtualizacao,
                ProductId = s.ProductId,
                ProductNome = s.ProductNome,
                ProductCodigo = s.ProductCodigo
            }),
            MinQuantityFilter = minQuantity,
            ShowLowStock = minQuantity.HasValue
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> LowStock(int minQuantity = 10)
    {
        var stocks = await _stockService.GetLowStockAsync(minQuantity);

        var viewModel = new StockListViewModel
        {
            Stocks = stocks.Select(s => new StockViewModel
            {
                Id = s.Id,
                QuantidadeAtual = s.QuantidadeAtual,
                DataUltimaAtualizacao = s.DataUltimaAtualizacao,
                ProductId = s.ProductId,
                ProductNome = s.ProductNome,
                ProductCodigo = s.ProductCodigo
            }),
            MinQuantityFilter = minQuantity,
            ShowLowStock = true
        };

        return View("Index", viewModel);
    }
}

