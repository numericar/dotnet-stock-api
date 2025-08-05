using System;
using StockAPI.Contexts;
using StockAPI.Models;
using StockAPI.Repositories.Intefaces;

namespace StockAPI.Repositories;

public class StockTransactionRepository : Repository<StockTransaction>, IStockTransactionRepository
{
    public StockTransactionRepository(AppDbContext mySQLDbContext) : base(mySQLDbContext)
    {
    }
}
