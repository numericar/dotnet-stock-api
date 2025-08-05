namespace StockAPI.DTOs.Requests;

public class RqStopLossDTO
{
    public int UserId { get; set; }
    public int TransactionId { get; set; }
    public decimal Price { get; set; }
}
