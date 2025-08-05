using StockAPI.Contexts;
using StockAPI.Models;
using StockAPI.Repositories.Intefaces;

namespace StockAPI.Repositories;

public class StopLossRepository : Repository<StopLoss>, IStopLossRepository
{
    public StopLossRepository(AppDbContext mySQLDbContext) : base(mySQLDbContext)
    {
    }
}
