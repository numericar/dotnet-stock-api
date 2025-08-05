using System;
using StockAPI.Models;

namespace StockAPI.Services.Interfaces;

public interface IStopLossService
{
    Task AddStopLoss(int userId, int transactionId, decimal price, int stockId);
    Task<bool> IsExistAsync(int transactionId);
    Task<List<StopLoss>> GetStopLossAsync(int stockId, decimal currentPrice);
    Task UpdateStopLoss(List<StopLoss> stopLosses);
}
