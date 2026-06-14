namespace MyTCGBinder.Application.DTOs.Responses;

public class TcgCardResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty; 
    public string Number { get; set; } = string.Empty;  
    public string SetId { get; set; } = string.Empty;     
    public string SetName { get; set; } = string.Empty;   
    public string Rarity { get; set; } = string.Empty;
    public string ImageSmall { get; set; } = string.Empty;
    public string ImageLarge { get; set; } = string.Empty;
}