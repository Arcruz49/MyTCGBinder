using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Exceptions;
 
namespace MyTCGBinder.Application.UseCases;
 
public class SearchCardsUseCase(ITcgService tcgService) : ISearchCardsUseCase
{
    public async Task<IEnumerable<TcgCardResponse>> ExecuteAsync(string? name, string? setId)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(setId))
            throw new ValidationException("Informe ao menos um filtro: name ou setId.");
 
        return await tcgService.SearchAsync(name, setId);
    }
}