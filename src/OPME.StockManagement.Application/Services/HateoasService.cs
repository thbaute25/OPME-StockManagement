using Microsoft.AspNetCore.Http;
using OPME.StockManagement.Application.DTOs;

namespace OPME.StockManagement.Application.Services;

public class HateoasService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HateoasService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return "http://localhost:5002";

        return $"{request.Scheme}://{request.Host}";
    }

    public List<Link> GetProductLinks(Guid productId)
    {
        var baseUrl = GetBaseUrl();
        return new List<Link>
        {
            new Link { Href = $"{baseUrl}/api/products/{productId}", Rel = "self", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/products", Rel = "collection", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/products", Rel = "create", Method = "POST" },
            new Link { Href = $"{baseUrl}/api/products/{productId}", Rel = "update", Method = "PUT" },
            new Link { Href = $"{baseUrl}/api/products/{productId}", Rel = "delete", Method = "DELETE" },
            new Link { Href = $"{baseUrl}/api/products/{productId}/toggle-status", Rel = "toggle-status", Method = "PATCH" },
            new Link { Href = $"{baseUrl}/api/stock/product/{productId}", Rel = "stock", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/products/search", Rel = "search", Method = "POST" }
        };
    }

    public List<Link> GetSupplierLinks(Guid supplierId)
    {
        var baseUrl = GetBaseUrl();
        return new List<Link>
        {
            new Link { Href = $"{baseUrl}/api/suppliers/{supplierId}", Rel = "self", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/suppliers", Rel = "collection", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/suppliers", Rel = "create", Method = "POST" },
            new Link { Href = $"{baseUrl}/api/suppliers/{supplierId}", Rel = "update", Method = "PUT" },
            new Link { Href = $"{baseUrl}/api/suppliers/{supplierId}", Rel = "delete", Method = "DELETE" },
            new Link { Href = $"{baseUrl}/api/suppliers/{supplierId}/toggle-status", Rel = "toggle-status", Method = "PATCH" },
            new Link { Href = $"{baseUrl}/api/suppliers/search", Rel = "search", Method = "POST" },
            new Link { Href = $"{baseUrl}/api/supplierconfigurations/supplier/{supplierId}", Rel = "configurations", Method = "GET" }
        };
    }

    public List<Link> GetStockLinks(Guid stockId, Guid productId)
    {
        var baseUrl = GetBaseUrl();
        return new List<Link>
        {
            new Link { Href = $"{baseUrl}/api/stock/product/{productId}", Rel = "self", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/stock", Rel = "collection", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/stock/product/{productId}/add", Rel = "add-stock", Method = "POST" },
            new Link { Href = $"{baseUrl}/api/stock/product/{productId}/reduce", Rel = "reduce-stock", Method = "POST" },
            new Link { Href = $"{baseUrl}/api/stock/low-stock", Rel = "low-stock", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/stock/search", Rel = "search", Method = "POST" },
            new Link { Href = $"{baseUrl}/api/products/{productId}", Rel = "product", Method = "GET" }
        };
    }

    public List<Link> GetCollectionLinks(string resourceName)
    {
        var baseUrl = GetBaseUrl();
        var resourcePath = resourceName.ToLower();
        return new List<Link>
        {
            new Link { Href = $"{baseUrl}/api/{resourcePath}", Rel = "self", Method = "GET" },
            new Link { Href = $"{baseUrl}/api/{resourcePath}", Rel = "create", Method = "POST" },
            new Link { Href = $"{baseUrl}/api/{resourcePath}/search", Rel = "search", Method = "POST" }
        };
    }
}

