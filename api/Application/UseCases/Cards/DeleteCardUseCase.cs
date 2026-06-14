using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
 
namespace MyTCGBinder.Application.UseCases;
public class DeleteCardUseCase(
    IUserCardRepository userCardRepository,
    IUnitOfWork unitOfWork) : IDeleteCardUseCase
{
    public async Task ExecuteAsync(Guid userId, Guid cardId)
    {
        var card = await userCardRepository.GetByIdAsync(cardId);
 
        if (card.UserId != userId)
            throw new ForbiddenException("Acesso negado");
 
        userCardRepository.DeleteAsync(card);
        await unitOfWork.SaveChangesAsync();
    }
}