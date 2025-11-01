using Microsoft.AspNetCore.Mvc;

namespace OPME.StockManagement.WebAPI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "OPME Stock Management";
        return View();
    }

    public IActionResult Error()
    {
        ViewData["Title"] = "Erro";
        return View();
    }
}

