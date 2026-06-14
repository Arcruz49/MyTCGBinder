using MyTCGBinder.Domain.Enums;
 
namespace MyTCGBinder.Application.DTOs.Responses;
 
public class CardResponse
{
    public Guid Id { get; set; }
    public string TcgCardId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string SetId { get; set; } = string.Empty;
    public string SetName { get; set; } = string.Empty;
    public string Rarity { get; set; } = string.Empty;
    public string ImageSmall { get; set; } = string.Empty;
    public string ImageLarge { get; set; } = string.Empty;
    public CardVariant Variant { get; set; }
    public int Quantity { get; set; }
}