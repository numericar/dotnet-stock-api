using System;

namespace StockAPI.DTOs.Requests;

public class RqBuyStockDTO
{
    public int UserId { get; set; }
    public required string StockName { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
