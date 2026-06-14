using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Exceptions;
 
namespace MyTCGBinder.Application.UseCases;
public class GetSetsUseCase(ITcgService tcgService) : IGetSetsUseCase
{
    public async Task<IEnumerable<TcgSetResponse>> ExecuteAsync()
    {
        return await tcgService.GetSetsAsync();
    }
}