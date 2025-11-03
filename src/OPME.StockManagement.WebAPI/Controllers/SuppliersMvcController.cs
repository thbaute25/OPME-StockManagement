using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OPME.StockManagement.Application.DTOs;
using OPME.StockManagement.Application.Services;
using OPME.StockManagement.Domain.Interfaces;
using OPME.StockManagement.WebAPI.ViewModels;

namespace OPME.StockManagement.WebAPI.Controllers;

public class SuppliersMvcController : Controller
{
    private readonly SupplierService _supplierService;
    private readonly ISupplierRepository _supplierRepository;

    public SuppliersMvcController(
        SupplierService supplierService,
        ISupplierRepository supplierRepository)
    {
        _supplierService = supplierService;
        _supplierRepository = supplierRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm)
    {
        var suppliers = await _supplierService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            suppliers = suppliers.Where(s =>
                s.Nome.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.Cnpj.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var viewModel = new SupplierListViewModel
        {
            Suppliers = suppliers.Select(s => new SupplierViewModel
            {
                Id = s.Id,
                Nome = s.Nome,
                Cnpj = s.Cnpj,
                Telefone = s.Telefone,
                Email = s.Email,
                Ativo = s.Ativo,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            }),
            SearchTerm = searchTerm
        };

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        var viewModel = new SupplierViewModel
        {
            Id = supplier.Id,
            Nome = supplier.Nome,
            Cnpj = supplier.Cnpj,
            Telefone = supplier.Telefone,
            Email = supplier.Email,
            Ativo = supplier.Ativo,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateSupplierViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupplierViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var dto = new CreateSupplierDto
        {
            Nome = viewModel.Nome,
            Cnpj = viewModel.Cnpj,
            Telefone = viewModel.Telefone,
            Email = viewModel.Email
        };

        try
        {
            await _supplierService.CreateAsync(dto);
            TempData["SuccessMessage"] = "Fornecedor criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var supplier = await _supplierService.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }

        var viewModel = new EditSupplierViewModel
        {
            Id = supplier.Id,
            Nome = supplier.Nome,
            Cnpj = supplier.Cnpj,
            Telefone = supplier.Telefone,
            Email = supplier.Email
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSupplierViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var dto = new CreateSupplierDto
        {
            Nome = viewModel.Nome,
            Cnpj = viewModel.Cnpj,
            Telefone = viewModel.Telefone,
            Email = viewModel.Email
        };

        try
        {
            await _supplierService.UpdateAsync(viewModel.Id, dto);
            TempData["SuccessMessage"] = "Fornecedor atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(viewModel);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _supplierService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Fornecedor excluído com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        try
        {
            await _supplierService.ToggleStatusAsync(id);
            TempData["SuccessMessage"] = "Status do fornecedor alterado com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}

