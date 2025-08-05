using System;

namespace StockAPI.DTOs.Requests;

public class RqSellStockDTO
{
    public int UserId { get; set; }
    public int TransactionId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
