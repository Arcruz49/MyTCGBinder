using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
 
namespace MyTCGBinder.Application.UseCases;
public class UpdateCardQuantityUseCase(
    IUserCardRepository userCardRepository,
    IUnitOfWork unitOfWork) : IUpdateCardQuantityUseCase
{
    public async Task ExecuteAsync(Guid userId, Guid cardId, UpdateCardQuantityRequest request)
    {
        var card = await userCardRepository.GetByIdAsync(cardId);
 
        if (card.UserId != userId)
            throw new ForbiddenException("Acesso negado");
 
        if (request.Action == "increment")
        {
            card.Quantity++;
        }
        else if (request.Action == "decrement")
        {
            if (card.Quantity <= 1)
                throw new ValidationException("Quantidade não pode ser menor que 1. Use DELETE para remover a carta.");
 
            card.Quantity--;
        }
        else
        {
            throw new ValidationException("Action inválida. Use 'increment' ou 'decrement'.");
        }
 
        userCardRepository.UpdateAsync(card);
        await unitOfWork.SaveChangesAsync();
    }
}