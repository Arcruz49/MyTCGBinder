using MyTCGBinder.Domain.Enums;
 
namespace MyTCGBinder.Application.DTOs.Request;
 
public class AddCardRequest
{
    public string TcgCardId { get; set; } = string.Empty;
    public CardVariant Variant { get; set; } = CardVariant.Normal;
}