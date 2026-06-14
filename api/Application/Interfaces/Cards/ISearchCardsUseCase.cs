using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface ISearchCardsUseCase
{
    Task<IEnumerable<TcgCardResponse>> ExecuteAsync(string? name, string? setId);
}
