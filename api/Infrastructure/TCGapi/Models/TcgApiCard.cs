namespace MyTCGBinder.Infrastructure.TcgApi.Models;

public class TcgApiCard
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string? Rarity { get; set; }
    public TcgApiSet? Set { get; set; }
    public TcgApiCardImages? Images { get; set; }
}

public class TcgApiCardImages
{
    public string? Small { get; set; }
    public string? Large { get; set; }
}