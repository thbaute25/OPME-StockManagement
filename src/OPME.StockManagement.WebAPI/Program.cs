using Microsoft.EntityFrameworkCore;
using OPME.StockManagement.Application.Services;
using OPME.StockManagement.Infrastructure;
using OPME.StockManagement.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Infrastructure (DbContext + Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Add Application Services
builder.Services.AddScoped<SupplierService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StockService>();

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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

