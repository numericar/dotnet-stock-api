using System;

namespace StockAPI.DTOs.Responses;

public class RsStockSellDTO
{
    public required string Name { get; set; }
    public required int Quantity { get; set; }
    public required decimal Price { get; set; }
}
