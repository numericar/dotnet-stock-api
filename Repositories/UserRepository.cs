using StockAPI.Contexts;
using StockAPI.Models;

namespace StockAPI.Repositories.Intefaces;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext mySQLDbContext) : base(mySQLDbContext)
    {
    }
}
