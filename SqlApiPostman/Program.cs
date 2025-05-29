using Microsoft.EntityFrameworkCore;
using SqlApiPostman.Data;
using System.Reflection;
using Serilog;
using Serilog.Events;
using SqlApiPostman.Models.Entities;
using SqlApiPostman.Repos.IRepo;
using SqlApiPostman.Repos.Repo;
//using SqlApiPostman.Repos.IRepo;
//using SqlApiPostman.Repos.Repo;



Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .WriteTo.File("logs/Items-.txt", rollingInterval: RollingInterval.Day)
               .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
// Configure Serilog for logging
builder.Host.UseSerilog();
// Add services to the container.

// Add AutoMapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
// Register the DbContext with dependency injection
builder.Services.AddDbContext<MyAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();

builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var productRepo = services.GetRequiredService<IProductRepo>();
        var categoryRepo = services.GetRequiredService<ICategoryRepo>();

        var products = productRepo.GetAllProductsAsync().Result;
        var categories = categoryRepo.GetAllCategoriesAsync().Result;

        Log.Information($"Application started with {products.Count()} products and {categories.Count()} categories.");
        Console.WriteLine($"Application started with {products.Count()} products and {categories.Count()} categories.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while retrieving product and category counts");
    }
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "API is running. Check console for product and category counts.");
app.Run();
