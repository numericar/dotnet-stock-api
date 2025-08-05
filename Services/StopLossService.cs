using System;
using Microsoft.EntityFrameworkCore;
using StockAPI.Models;
using StockAPI.Repositories.Intefaces;
using StockAPI.Services.Interfaces;

namespace StockAPI.Services;

public class StopLossService : IStopLossService
{
    private readonly IStopLossRepository stopLossRepository;

    public StopLossService(IStopLossRepository stopLossRepository)
    {
        this.stopLossRepository = stopLossRepository;
    }

    public async Task AddStopLoss(int userId, int transactionId, decimal price, int stockId)
    {
        try
        {
            StopLoss stopLoss = new StopLoss()
            {
                UserId = userId,
                TransactionId = transactionId,
                IsActive = true,
                StockId = stockId,
                Price = price
            };

            await this.stopLossRepository.Add(stopLoss);
            await this.stopLossRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<StopLoss>> GetStopLossAsync(int stockId, decimal currentPrice)
    {
        try
        {
            return await this.stopLossRepository.Find(x => x.StockId == stockId && x.Price >= currentPrice).ToListAsync();
        }   
        catch
        {
            throw;
        }
    }

    public async Task<bool> IsExistAsync(int transactionId)
    {
        try
        {
            return (await this.stopLossRepository.Find(x => x.TransactionId == transactionId && x.IsActive == true).FirstOrDefaultAsync()) != null;
        }
        catch
        {
            throw;
        }
    }

    public async Task UpdateStopLoss(List<StopLoss> stopLosses)
    {
        try
        {
            this.stopLossRepository.DbSet.UpdateRange(stopLosses);
            await this.stopLossRepository.SaveChangeAsync();
        }
        catch
        {
            throw;
        }
    }
}
