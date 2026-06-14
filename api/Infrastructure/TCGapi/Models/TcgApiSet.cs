namespace MyTCGBinder.Infrastructure.TcgApi.Models;

public class TcgApiSet
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Series { get; set; }
    public TcgApiSetImages? Images { get; set; }
}

public class TcgApiSetImages
{
    public string? Logo { get; set; }
}