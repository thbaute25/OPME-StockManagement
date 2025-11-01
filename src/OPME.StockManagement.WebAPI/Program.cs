using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Application.Services;
using OPME.StockManagement.Application.Validators;
using OPME.StockManagement.Infrastructure;
using OPME.StockManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(CreateSupplierDtoValidator).Assembly);

// Add Infrastructure (DbContext + Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application Services
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StockService>();
builder.Services.AddScoped<SupplierConfigurationService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OPME Stock Management API");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

// Configure default MVC routes first (conventional routing)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// MVC Routes for Products
app.MapControllerRoute(
    name: "products",
    pattern: "Products/{action=Index}/{id?}",
    defaults: new { controller = "ProductsMvc", action = "Index" });

// MVC Routes for Suppliers
app.MapControllerRoute(
    name: "suppliers",
    pattern: "Suppliers/{action=Index}/{id?}",
    defaults: new { controller = "SuppliersMvc", action = "Index" });

// MVC Routes for Stock
app.MapControllerRoute(
    name: "stock",
    pattern: "Stock/{action=Index}",
    defaults: new { controller = "StockMvc", action = "Index" });

// Map API controllers (those with [ApiController] attribute)
app.MapControllers();

// Custom Routes - Rotas personalizadas (devem vir depois das rotas de controller)
// Rotas amigáveis em português como aliases para recursos da API
// Essas rotas redirecionam para os endpoints da API mantendo compatibilidade
app.MapGet("/produtos", () => Results.Redirect("/api/Products", permanent: false))
    .WithName("produtos")
    .WithDisplayName("Listar Produtos")
    .WithTags("Produtos");

app.MapGet("/fornecedores", () => Results.Redirect("/api/Suppliers", permanent: false))
    .WithName("fornecedores")
    .WithDisplayName("Listar Fornecedores")
    .WithTags("Fornecedores");

app.MapGet("/estoque", () => Results.Redirect("/api/Stock", permanent: false))
    .WithName("estoque")
    .WithDisplayName("Listar Estoque")
    .WithTags("Estoque");

app.MapGet("/estoque/baixo", () => Results.Redirect("/api/Stock/low-stock", permanent: false))
    .WithName("estoque-baixo")
    .WithDisplayName("Estoque Baixo")
    .WithTags("Estoque");

app.MapGet("/produtos/ativos", () => Results.Redirect("/api/Products/active", permanent: false))
    .WithName("produtos-ativos")
    .WithDisplayName("Produtos Ativos")
    .WithTags("Produtos");

// Rotas personalizadas com parâmetros usando Endpoint Routing
app.MapGet("/produtos/{id:guid}", (Guid id) => Results.Redirect($"/api/Products/{id}", permanent: false))
    .WithName("produto-detalhes")
    .WithDisplayName("Detalhes do Produto")
    .WithTags("Produtos");

app.MapGet("/fornecedores/{id:guid}", (Guid id) => Results.Redirect($"/api/Suppliers/{id}", permanent: false))
    .WithName("fornecedor-detalhes")
    .WithDisplayName("Detalhes do Fornecedor")
    .WithTags("Fornecedores");

app.MapGet("/produtos/{productId:guid}/estoque", (Guid productId) => Results.Redirect($"/api/Stock/product/{productId}", permanent: false))
    .WithName("estoque-produto")
    .WithDisplayName("Estoque do Produto")
    .WithTags("Estoque");

// Rotas para configurações
app.MapGet("/configuracoes", () => Results.Redirect("/api/SupplierConfigurations", permanent: false))
    .WithName("configuracoes")
    .WithDisplayName("Configurações de Fornecedores")
    .WithTags("Configurações");

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Seed(context);
}

app.Run();

