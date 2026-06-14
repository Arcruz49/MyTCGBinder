using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;

namespace MyTCGBinder.Application.UseCases;

public class SearchCardsUseCase(ITCGCardRepository tcgCardRepository) : ISearchCardsUseCase
{
    public async Task<IEnumerable<TcgCardResponse>> ExecuteAsync(string? name, string? setId)
    {
        if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(setId))
            throw new ValidationException("Informe ao menos um filtro: name ou setId.");

        var cards = await tcgCardRepository.SearchAsync(name, setId);

        return cards.Select(c => new TcgCardResponse
        {
            Id = c.Id,
            Name = c.Name,
            Number = c.Number,
            SetId = c.SetId,
            SetName = c.SetName,
            Rarity = c.Rarity,
            ImageSmall = c.ImageSmall,
            ImageLarge = c.ImageLarge
        });
    }
}
