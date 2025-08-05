using System;
using Microsoft.EntityFrameworkCore;
using StockAPI.Models;

namespace StockAPI.Contexts;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<StockTransaction> StockTransactions { get; set; }
    public DbSet<StopLoss> StopLosses { get; set; }
}
