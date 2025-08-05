using System;

namespace StockAPI.DTOs.Services;

public class StockTransactionSell
{
    public int StockId { get; set; }
    public int TransactionId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
