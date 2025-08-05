using System;

namespace StockAPI.DTOs.Services;

public class StockSellDTO
{
    public required string Name { get; set; }
    public required int Quantity { get; set; }
    public required decimal Price { get; set; }
}
