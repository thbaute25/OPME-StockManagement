using Microsoft.AspNetCore.Mvc;

namespace OPME.StockManagement.WebAPI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Início";
        return View();
    }
}

