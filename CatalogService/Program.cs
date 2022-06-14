using CatalogService.Data;
using CatalogService.Models;
using CatalogService.Repos;
using CatalogService.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Console.WriteLine("--> Using PostgreSQL Database");
builder.Services.AddDbContext<CatalogDbContext>(opt =>
//opt.UseSqlServer(@"Server=.\SQLEXPRESS;Database=WebMartCatalog;Trusted_Connection=True;"));
opt.UseNpgsql(@"Host=localhost;Port=5432;Database=WebMartCatalog;Username=postgres;Password=7628"));
builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<ISubCategoryRepo, SubCategoryRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
