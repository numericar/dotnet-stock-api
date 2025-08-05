using System;
using Microsoft.EntityFrameworkCore;
using StockAPI.DTOs.Responses;
using StockAPI.DTOs.Services;
using StockAPI.Models;
using StockAPI.Repositories.Intefaces;
using StockAPI.Services.Interfaces;

namespace StockAPI.Services;

public class StockTransactionService : IStockTransactionService
{
    private readonly IStockTransactionRepository stockTransactionRepository;
    private readonly IStockRepository stockRepository;

    public StockTransactionService(IStockTransactionRepository stockTransactionRepository, IStockRepository stockRepository)
    {
        this.stockTransactionRepository = stockTransactionRepository;
        this.stockRepository = stockRepository;
    }

    public async Task<StockTransaction?> GetTransactionAsync(int transactionId)
    {
        try
        {
            return await this.stockTransactionRepository.Find(x => x.Id == transactionId).OrderBy(x => x.CreatedAt).FirstOrDefaultAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<StockTransaction>?> GetTransactionAsync(int stockId, decimal price)
    {
        try
        {
            return await this.stockTransactionRepository.Find(x => x.StockId == stockId && x.Price == price).OrderBy(x => x.CreatedAt).ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task DecreaseQuantityAsync(List<StockTransaction> stockTransactions)
    {
        try
        {
            this.stockTransactionRepository.DbSet.UpdateRange(stockTransactions);
            await this.stockTransactionRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task CreateStockTransactionAsync(int userId, int stockId, decimal price, int quantity, string actionType)
    {
        try
        {
            StockTransaction stockTransaction = new StockTransaction()
            {
                UserId = userId,
                StockId = stockId,
                ActionType = actionType,
                Quantity = quantity,
                Price = price,
                CreatedAt = DateTime.Now
            };

            await this.stockTransactionRepository.Add(stockTransaction);
            await this.stockTransactionRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task UpdateQuantityAsync(int transactionId, int quantity)
    {
        try
        {
            StockTransaction? stockTransaction = await this.stockTransactionRepository.Find(x => x.Id == transactionId).FirstOrDefaultAsync();

            if (stockTransaction == null) throw new Exception("Transaction id is invalid");

            stockTransaction.Quantity = quantity;

            this.stockTransactionRepository.Update(stockTransaction);
            await this.stockTransactionRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<UserStockDTO>> GetBuyStockTransactionsByUserIdAsync(int userId)
    {
        try
        {
            return await this.stockTransactionRepository.DbSet
                .Where(x => x.UserId == userId && x.Quantity > 0 && x.ActionType.Equals("buy"))
                .Join(
                    this.stockRepository.DbSet,
                    stockTrans => stockTrans.StockId,
                    stock => stock.Id,
                    (stockTrans, stock) => new UserStockDTO()
                    {
                        StockId = stock.Id,
                        TransactionId = stockTrans.Id,
                        ActionType = stockTrans.ActionType,
                        Name = stock.Name,
                        Price = stockTrans.Price,
                        CurrentStockPrice = stock.CurrentPrice,
                        Quantity = stockTrans.Quantity
                    })
                .ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<RsUserSellStock>> GetSellStockTransactionsByUserIdAsync(int userId)
    {
        try
        {
            return await this.stockTransactionRepository.DbSet
                .Where(x => x.UserId == userId && x.Quantity > 0 && x.ActionType.Equals("sell"))
                .Join(
                    this.stockRepository.DbSet,
                    stockTrans => stockTrans.StockId,
                    stock => stock.Id,
                    (stockTrans, stock) => new RsUserSellStock()
                    {
                        StockId = stock.Id,
                        TransactionId = stockTrans.Id,
                        Name = stock.Name,
                        Price = stockTrans.Price,
                        Quantity = stockTrans.Quantity,
                        CreatedAt = stockTrans.CreatedAt
                    })
                .ToListAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task RemoveTransaction(int transactionId)
    {
        try
        {
            StockTransaction? stockTransaction = await this.stockTransactionRepository.Find(x => x.Id == transactionId).FirstOrDefaultAsync();
            if (stockTransaction == null) throw new Exception("Transaction id is invalid");
            
            this.stockTransactionRepository.Remove(stockTransaction);
            await this.stockTransactionRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }
}
