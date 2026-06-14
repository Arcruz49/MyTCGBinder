namespace MyTCGBinder.Infrastructure.TcgApi.Models;

public class TcgApiListResponse<T>
{
    public List<T>? Data { get; set; }
}

public class TcgApiSingleResponse<T>
{
    public T? Data { get; set; }
}