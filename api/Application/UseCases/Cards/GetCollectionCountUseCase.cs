using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
 
namespace MyTCGBinder.Application.UseCases;
public class GetCollectionCountUseCase(IUserCardRepository userCardRepository) : IGetCollectionCountUseCase
{
    public async Task<int> ExecuteAsync(Guid userId)
    {
        return await userCardRepository.GetTotalCountByUserIdAsync(userId);
    }
}