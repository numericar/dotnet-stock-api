using System;

namespace StockAPI.DTOs.Responses;

public class RsStockDTO
{
    public required string Name { get; set; }
    public required decimal Value { get; set; }
}
