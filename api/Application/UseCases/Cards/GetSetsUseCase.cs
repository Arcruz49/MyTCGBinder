using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Interfaces;

namespace MyTCGBinder.Application.UseCases;
public class GetSetsUseCase(ITCGCardRepository tcgCardRepository) : IGetSetsUseCase
{
    public async Task<IEnumerable<TcgSetResponse>> ExecuteAsync()
    {
        var sets = await tcgCardRepository.GetAllSetsAsync();

        return sets.Select(c => new TcgSetResponse
        {
            Id = c.SetId,
            Name = c.SetName,
            Series = c.Series,
            Logo = string.Empty
        });
    }
}
