using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockAPI.DTOs;
using StockAPI.DTOs.Responses;
using StockAPI.DTOs.Services;
using StockAPI.Models;
using StockAPI.Services.Interfaces;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IStockTransactionService stockTransactionService;

        public UsersController(IStockTransactionService stockTransactionService)
        {
            this.stockTransactionService = stockTransactionService;
        }

        [HttpGet("{userId}/stocks")]
        public async Task<IActionResult> GetStocks(int userId)
        {
            try
            {
                List<UserStockDTO> stockTransactions = await this.stockTransactionService.GetBuyStockTransactionsByUserIdAsync(userId);
                List<RsUserStockItem> holdStocks = new List<RsUserStockItem>();
                foreach (UserStockDTO stockTransaction in stockTransactions)
                {
                    holdStocks.Add(new RsUserStockItem()
                    {
                        StockId = stockTransaction.StockId,
                        TransactionId = stockTransaction.TransactionId,
                        Name = stockTransaction.Name,
                        Value = stockTransaction.Price,
                        Profit = ((stockTransaction.CurrentStockPrice - stockTransaction.Price) / stockTransaction.Price) * 100,
                        Quantity = stockTransaction.Quantity
                    });
                }

                List<RsUserSellStock> sellStocks = await this.stockTransactionService.GetSellStockTransactionsByUserIdAsync(userId);

                RsUserStock result = new RsUserStock();
                result.HoldStocks = holdStocks;
                result.SellStocks = sellStocks;

                return Ok(new BaseResponse<object>(true, "Successful", result));
            }
            catch (Exception ex)
            {
                
                return Ok(new BaseResponse<object>(false, ex.Message, null));
            }
        }

        [HttpGet("health-check")]
        public IActionResult HealthCheck()
        {
            return Ok(new BaseResponse<object>(true, "Successful", null));
        }
    }
}
