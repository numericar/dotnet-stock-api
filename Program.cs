using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using StockAPI.Contexts;
using StockAPI.Repositories;
using StockAPI.Repositories.Intefaces;
using StockAPI.Services;
using StockAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<AppDbContext>(option =>
{
    string? connectionString = builder.Configuration.GetConnectionString("Default");

    if (connectionString == null) throw new Exception("Connection string not found");

    option.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOption => mysqlOption.SchemaBehavior(MySqlSchemaBehavior.Ignore));
});

// add repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();
builder.Services.AddScoped<IStopLossRepository, StopLossRepository>();

// add services
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockTransactionService, StockTransactionService>();
builder.Services.AddScoped<IStopLossService, StopLossService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();
app.Run();