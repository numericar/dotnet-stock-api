using System;
using StockAPI.DTOs.Responses;
using StockAPI.DTOs.Services;
using StockAPI.Models;

namespace StockAPI.Services.Interfaces;

public interface IStockService
{
    Task<List<StockDTO>> GetStocksAsync();
    Task<Stock?> GetStockByNameAsync(string stockName);
    Task<StockTransaction?> GetStockTransactionByIdAsync(int transactionId);
    Task CreateStockTransaction(int userId, int stockId, decimal price, int quantity);
    Task DecreaseStockTransaction(int transactionId, int quantity);
    Task<List<RsStockSellDTO>> GetStockSellAsyncs(int stockId);
    Task UpdatePrice(int stockId, decimal price);
}
