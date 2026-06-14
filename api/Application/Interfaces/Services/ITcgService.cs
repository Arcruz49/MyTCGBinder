using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface ITcgService
{
    Task<IEnumerable<TcgCardResponse>> SearchAsync(string? name, string? setId);
    Task<TcgCardResponse?> GetByIdAsync(string tcgCardId);
    Task<IEnumerable<TcgSetResponse>> GetSetsAsync();
}