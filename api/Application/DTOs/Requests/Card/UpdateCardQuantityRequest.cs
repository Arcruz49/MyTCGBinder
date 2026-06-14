using MyTCGBinder.Domain.Enums;
 
namespace MyTCGBinder.Application.DTOs.Request;
 
public class UpdateCardQuantityRequest
{
    public string Action { get; set; } = string.Empty; // "increment" "decrement"
}
 