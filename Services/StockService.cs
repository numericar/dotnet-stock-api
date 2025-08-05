using System;
using Microsoft.EntityFrameworkCore;
using StockAPI.DTOs.Responses;
using StockAPI.DTOs.Services;
using StockAPI.Models;
using StockAPI.Repositories.Intefaces;
using StockAPI.Services.Interfaces;

namespace StockAPI.Services;

public class StockService : IStockService
{
    private readonly IStockRepository stockRepository;
    private readonly IStockTransactionRepository stockTransactionRepository;

    public StockService(IStockRepository stockRepository, IStockTransactionRepository stockTransactionRepository)
    {
        this.stockRepository = stockRepository;
        this.stockTransactionRepository = stockTransactionRepository;
    }

    public async Task<List<StockDTO>> GetStocksAsync()
    {
        try
        {
            return await this.stockRepository.DbSet.Select(x => new StockDTO()
            {
                Name = x.Name,
                Value = x.CurrentPrice
            }).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<StockTransaction?> GetStockTransactionByIdAsync(int transactionId)
    {
        try
        {
            return await this.stockTransactionRepository.Find(x => x.Id == transactionId).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task CreateStockTransaction(int userId, int stockId, decimal price, int quantity)
    {
        try
        {
            StockTransaction stockTransaction = new StockTransaction()
            {
                UserId = userId,
                StockId = stockId,
                ActionType = "buy",
                Quantity = quantity,
                Price = price,
                CreatedAt = DateTime.Now
            };

            await this.stockTransactionRepository.Add(stockTransaction);
            await this.stockRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task DecreaseStockTransaction(int transactionId, int quantity)
    {
        try
        {
            StockTransaction? stockTransaction = await this.stockTransactionRepository.Find(x => x.Id == transactionId).FirstOrDefaultAsync();

            if (stockTransaction == null) throw new Exception("Stock transaction not found");

            stockTransaction.Quantity -= quantity;

            this.stockTransactionRepository.Update(stockTransaction);
            await this.stockTransactionRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<RsStockSellDTO>> GetStockSellAsyncs(int stockId)
    {
        try
        {
            List<StockSellDTO> stockSells = await this.stockTransactionRepository
                .Find(x => x.StockId == stockId && x.ActionType.Equals("sell") && x.Quantity > 0)
                .Join(
                    this.stockRepository.DbSet,
                    stockTrans => stockTrans.StockId,
                    stock => stock.Id,
                    (stockTras, stock) => new StockSellDTO()
                    {
                        Name = stock.Name,
                        Price = stockTras.Price,
                        Quantity = stockTras.Quantity
                    })
                .ToListAsync();

            List<RsStockSellDTO> result = new List<RsStockSellDTO>();
            foreach (StockSellDTO stockSell in stockSells)
            {
                bool isDuplicatePrice = false;

                foreach (RsStockSellDTO rsStockSellDTO in result)
                {
                    if (stockSell.Price == rsStockSellDTO.Price)
                    {
                        rsStockSellDTO.Quantity += stockSell.Quantity;
                        isDuplicatePrice = true;
                        break;
                    }
                }

                if (!isDuplicatePrice)
                {
                    RsStockSellDTO rsStockSellDTO = new RsStockSellDTO()
                    {
                        Name = stockSell.Name,
                        Price = stockSell.Price,
                        Quantity = stockSell.Quantity
                    };

                    result.Add(rsStockSellDTO);
                }
            }

            return result;
        }
        catch
        {
            throw;
        }
    }

    public async Task<Stock?> GetStockByNameAsync(string stockName)
    {
        try
        {
            return await this.stockRepository.Find(x => x.Name.Equals(stockName)).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task UpdatePrice(int stockId, decimal price)
    {
        try
        {
            Stock? stock = await this.stockRepository.Find(x => x.Id == stockId).FirstOrDefaultAsync();

            if (stock == null) throw new Exception("Stock not found");

            stock.CurrentPrice = price;

            this.stockRepository.Update(stock);
            await this.stockRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }
}
