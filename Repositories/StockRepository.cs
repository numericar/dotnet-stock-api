using System;
using StockAPI.Contexts;
using StockAPI.Models;
using StockAPI.Repositories.Intefaces;

namespace StockAPI.Repositories;

public class StockRepository : Repository<Stock>, IStockRepository
{
    public StockRepository(AppDbContext mySQLDbContext) : base(mySQLDbContext)
    {
    }
}
