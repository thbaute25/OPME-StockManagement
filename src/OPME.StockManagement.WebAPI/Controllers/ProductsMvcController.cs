using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Services;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.WebAPI.ViewModels;

namespace OPME.StockManagement.WebAPI.Controllers;

public class ProductsMvcController : Controller
{
    private readonly ProductService _productService;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IBrandRepository _brandRepository;

    public ProductsMvcController(
        ProductService productService,
        ISupplierRepository supplierRepository,
        IBrandRepository brandRepository)
    {
        _productService = productService;
        _supplierRepository = supplierRepository;
        _brandRepository = brandRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, bool? showOnlyActive)
    {
        var products = await _productService.GetAllAsync();
        
        if (showOnlyActive == true)
        {
            products = products.Where(p => p.Ativo);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            products = products.Where(p => 
                p.NomeProduto.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                p.CodigoProduto.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var viewModel = new ProductListViewModel
        {
            Products = products.Select(p => new ProductViewModel
            {
                Id = p.Id,
                CodigoProduto = p.CodigoProduto,
                NomeProduto = p.NomeProduto,
                Ativo = p.Ativo,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                SupplierId = p.SupplierId,
                BrandId = p.BrandId,
                SupplierNome = p.SupplierNome,
                BrandNome = p.BrandNome,
                QuantidadeEstoque = p.QuantidadeEstoque
            }),
            SearchTerm = searchTerm,
            ShowOnlyActive = showOnlyActive
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            CodigoProduto = product.CodigoProduto,
            NomeProduto = product.NomeProduto,
            Ativo = product.Ativo,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt,
            SupplierId = product.SupplierId,
            BrandId = product.BrandId,
            SupplierNome = product.SupplierNome,
            BrandNome = product.BrandNome,
            QuantidadeEstoque = product.QuantidadeEstoque
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadViewBagData();
        return View(new CreateProductViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewBagData();
            return View(viewModel);
        }

        var dto = new CreateProductDto
        {
            CodigoProduto = viewModel.CodigoProduto,
            NomeProduto = viewModel.NomeProduto,
            SupplierId = viewModel.SupplierId,
            BrandId = viewModel.BrandId
        };

        try
        {
            await _productService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Produto criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadViewBagData();
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        var viewModel = new EditProductViewModel
        {
            Id = product.Id,
            CodigoProduto = product.CodigoProduto,
            NomeProduto = product.NomeProduto,
            SupplierId = product.SupplierId,
            BrandId = product.BrandId
        };

        await LoadViewBagData();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProductViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            await LoadViewBagData();
            return View(viewModel);
        }

        var dto = new CreateProductDto
        {
            CodigoProduto = viewModel.CodigoProduto,
            NomeProduto = viewModel.NomeProduto,
            SupplierId = viewModel.SupplierId,
            BrandId = viewModel.BrandId
        };

        try
        {
            await _productService.UpdateAsync(viewModel.Id, dto);
            TempData["SuccessMessage"] = "Produto atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            await LoadViewBagData();
            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _productService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Produto excluído com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task LoadViewBagData()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        var brands = await _brandRepository.GetAllAsync();

        ViewBag.Suppliers = new SelectList(suppliers, "Id", "Nome");
        ViewBag.Brands = new SelectList(brands, "Id", "Nome");
    }
}

