namespace OPME.StockManagement.Application.DTOs;

public class Link
{
    public string Href { get; set; } = string.Empty;
    public string Rel { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
}

public class LinkedResource<T>
{
    public T Data { get; set; } = default!;
    public List<Link> Links { get; set; } = new List<Link>();
}

