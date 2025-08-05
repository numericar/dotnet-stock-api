namespace StockAPI.DTOs;

public class BaseResponse<T>
{
    public BaseResponse()
    {
    }

    public BaseResponse(bool isSuccess, string message, T? data)
    {
        this.IsSuccess = isSuccess;
        this.Message = message;
        this.Data = data;
    }

    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
}
