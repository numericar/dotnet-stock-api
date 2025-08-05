using System;

namespace StockAPI.DTOs.Services;

public class UserStockDTO
{
    public int StockId { get; set; }
    public int TransactionId { get; set; }
    public required string ActionType { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public required decimal CurrentStockPrice { get; set; }
    public required int Quantity { get; set; }
}
