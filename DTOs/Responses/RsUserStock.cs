using System;

namespace StockAPI.DTOs.Responses;

public class RsUserStock
{
    public List<RsUserStockItem>? HoldStocks { get; set; }
    public List<RsUserSellStock>? SellStocks { get; set; }
}

public class RsUserStockItem
{
    public int StockId { get; set; }
    public int TransactionId { get; set; }
    public required string Name { get; set; }
    public required decimal Value { get; set; }
    public required decimal Profit { get; set; }
    public int Quantity { get; set; }
}

public class RsUserSellStock
{
    public int StockId { get; set; }
    public int TransactionId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
