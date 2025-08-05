using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockAPI.DTOs;
using StockAPI.DTOs.Requests;
using StockAPI.DTOs.Responses;
using StockAPI.DTOs.Services;
using StockAPI.Models;
using StockAPI.Services.Interfaces;

namespace StockAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStockService stockService;
        private readonly IStockTransactionService stockTransactionService;
        private readonly IStopLossService stopLossService;

        public StocksController(IStockService stockService, IStockTransactionService stockTransactionService, IStopLossService stopLossService)
        {
            this.stockService = stockService;
            this.stockTransactionService = stockTransactionService;
            this.stopLossService = stopLossService;
        }

        // ดึงข้อมูลหุ้นที่มีในระบบ
        [HttpGet]
        public async Task<IActionResult> GetStocks()
        {
            try
            {
                List<StockDTO> stocks = await this.stockService.GetStocksAsync();

                List<RsStockDTO> response = stocks.Select(x => new RsStockDTO()
                {
                    Name = x.Name,
                    Value = x.Value
                }).ToList();

                return Ok(new BaseResponse<List<RsStockDTO>>(true, "Successful", response));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>(false, ex.Message, null));
            }
        }

        // ดึงข้อมูลราคาหุ้นที่มีให้ซิ้อของหุ้นตัวนั้น
        [HttpGet("{stockName}/sells")]
        public async Task<IActionResult> GetStockSells(string stockName)
        {
            try
            {
                Stock? stock = await this.stockService.GetStockByNameAsync(stockName);
                if (stock == null) throw new Exception("Stock name is invalid");

                List<RsStockSellDTO> stockSellDTOs = await this.stockService.GetStockSellAsyncs(stock.Id);

                return Ok(new BaseResponse<object>(true, "Successful", stockSellDTOs));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>(false, ex.Message, null));
            }
        }


        // ซื้อหุ้น
        [HttpPost("buy")]
        public async Task<IActionResult> ActionBuy(RqBuyStockDTO dto)
        {
            try
            {
                Stock? stock = await this.stockService.GetStockByNameAsync(dto.StockName);
                if (stock == null) throw new Exception("Stock name is invalid");

                List<StockTransaction>? stockTransactions = await this.stockTransactionService.GetTransactionAsync(stock.Id, dto.Price);
                if (stockTransactions == null) throw new Exception("There is no price at which you would like to purchase");

                int currentQuantity = stockTransactions.Sum(x => x.Quantity);
                if (currentQuantity < dto.Quantity) throw new Exception("insufficient quantity");

                List<StockTransactionSell> stockTransactionSells = new List<StockTransactionSell>(); // สำหรับบันทึกว่าหุ้นที่ถูกวางขายตัวไหนจะถูกซื้อบ้าง
                foreach (StockTransaction stockTransactionItem in stockTransactions)
                {
                    if (dto.Quantity <= 0) break;

                    if (stockTransactionItem.Quantity >= dto.Quantity) // ถ้าจำนวนหุ้นในระบบ มีมากกว่าที่ผู้ใช้ขอซื้อ
                    {
                        stockTransactionSells.Add(new StockTransactionSell()
                        {
                            StockId = stockTransactionItem.StockId,
                            TransactionId = stockTransactionItem.Id,
                            Price = stockTransactionItem.Price,
                            Quantity = dto.Quantity
                        });

                        stockTransactionItem.Quantity -= dto.Quantity;
                        dto.Quantity = 0;
                    }
                    else if (stockTransactionItem.Quantity < dto.Quantity) // ถ้าจำนวนหุ้นในระบบมีน้อยกว่าที่ผู้ใช้ขอซื้อ
                    {
                        stockTransactionSells.Add(new StockTransactionSell()
                        {
                            StockId = stockTransactionItem.StockId,
                            TransactionId = stockTransactionItem.Id,
                            Price = stockTransactionItem.Price,
                            Quantity = stockTransactionItem.Quantity
                        });

                        dto.Quantity -= stockTransactionItem.Quantity;
                        stockTransactionItem.Quantity = 0;
                    }
                }

                List<StockTransaction> decreaseQuantities = new List<StockTransaction>();
                foreach (StockTransactionSell stockTransactionSell in stockTransactionSells)
                {
                    foreach (StockTransaction stockTransactionItem in stockTransactions)
                    {
                        if (stockTransactionSell.TransactionId == stockTransactionItem.Id)
                        {
                            decreaseQuantities.Add(stockTransactionItem);
                            break;
                        }
                    }
                }

                await this.stockTransactionService.DecreaseQuantityAsync(decreaseQuantities);
                await this.stockTransactionService.CreateStockTransactionAsync(dto.UserId, stock.Id, dto.Price, stockTransactionSells.Select(x => x.Quantity).Sum(), "buy");

                await this.stockService.UpdatePrice(stock.Id, dto.Price);
                
                // stop loss trigger
                List<StopLoss> stopLosses = await this.stopLossService.GetStopLossAsync(stock.Id, dto.Price);
                foreach (StopLoss stopLoss in stopLosses)
                {
                    StockTransaction? stockTransaction = await this.stockTransactionService.GetTransactionAsync(stopLoss.TransactionId);
                    if (stockTransaction == null) throw new Exception("Transaction not found");

                    await this.stockTransactionService.CreateStockTransactionAsync(
                        stopLoss.UserId,
                        stockTransaction.StockId,
                        stopLoss.Price,
                        stockTransaction.Quantity,
                        "sell");

                    stopLoss.IsActive = false;

                    await this.stockTransactionService.RemoveTransaction(stockTransaction.Id);
                }
                if (stopLosses.Count > 0) await this.stopLossService.UpdateStopLoss(stopLosses);


                return Ok(new BaseResponse<object>(true, "Successful", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>(false, ex.Message, null));
            }
        }

        // ขายหุ้น
        [HttpPost("sell")]
        public async Task<IActionResult> ActionSell(RqSellStockDTO dto)
        {
            try
            {
                StockTransaction? stockTransaction = await this.stockTransactionService.GetTransactionAsync(dto.TransactionId);
                if (stockTransaction == null) throw new Exception("Transaction not found");

                if (dto.Quantity > stockTransaction.Quantity) throw new Exception("insufficient quantity");

                int quantityRemaining = stockTransaction.Quantity - dto.Quantity;

                await this.stockTransactionService.CreateStockTransactionAsync(dto.UserId, stockTransaction.StockId, dto.Price, dto.Quantity, "sell");
                await this.stockTransactionService.UpdateQuantityAsync(dto.TransactionId, quantityRemaining);

                return Ok(new BaseResponse<object>(true, "Successful", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>(false, ex.Message, null));
            }
        }

        // เพิ่มจุด stop loss
        [HttpPost("stop-loss")]
        public async Task<IActionResult> AddStopLoss(RqStopLossDTO dto)
        {
            try
            {
                StockTransaction? stockTransaction = await this.stockTransactionService.GetTransactionAsync(dto.TransactionId);
                if (stockTransaction == null) throw new Exception("Transaction not found");

                if (!stockTransaction.ActionType.Equals("buy")) throw new Exception("Can add stop loss only buy transaction");

                if (dto.Price >= stockTransaction.Price) throw new Exception("Stop loss price should less than current price");

                if (await this.stopLossService.IsExistAsync(dto.TransactionId)) throw new Exception("Stop loss for this transaction is exist");

                await this.stopLossService.AddStopLoss(dto.UserId, dto.TransactionId, dto.Price, stockTransaction.StockId);

                return Ok(new BaseResponse<object>(true, "Successful", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new BaseResponse<object>(false, ex.Message, null));
            }
        }

        [HttpGet("health-check")]
        public IActionResult HealthCheck()
        {
            return Ok(new BaseResponse<object>(true, "Successful", null));
        }
    }
} 
