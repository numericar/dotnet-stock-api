using System;
using StockAPI.DTOs.Responses;
using StockAPI.DTOs.Services;
using StockAPI.Models;

namespace StockAPI.Services.Interfaces;

public interface IStockTransactionService
{
    Task<List<StockTransaction>?> GetTransactionAsync(int stockId, decimal price);
    Task DecreaseQuantityAsync(List<StockTransaction> stockTransactions);
    Task CreateStockTransactionAsync(int userId, int stockId, decimal price, int quantity, string actionType);
    Task<StockTransaction?> GetTransactionAsync(int transactionId);
    Task UpdateQuantityAsync(int transactionId, int quantity);
    Task<List<UserStockDTO>> GetBuyStockTransactionsByUserIdAsync(int userId);
    Task<List<RsUserSellStock>> GetSellStockTransactionsByUserIdAsync(int userId);
    Task RemoveTransaction(int transactionId);
}
