using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
 
namespace MyTCGBinder.Application.UseCases;
public class AddCardUseCase(
    IUserCardRepository userCardRepository,
    ITcgService tcgService,
    IUnitOfWork unitOfWork) : IAddCardUseCase
{
    public async Task<CardResponse> ExecuteAsync(Guid userId, AddCardRequest request)
    {
        var tcgCard = await tcgService.GetByIdAsync(request.TcgCardId)
            ?? throw new NotFoundException("Carta não encontrada na API");
 
        var existing = await userCardRepository.GetByTcgCardIdAndVariantAsync(userId, request.TcgCardId, request.Variant);
 
        if (existing is not null)
        {
            existing.Quantity++;
            userCardRepository.UpdateAsync(existing);
            await unitOfWork.SaveChangesAsync();
 
            return MapToResponse(existing);
        }
 
        var card = new UserCard
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TcgCardId = tcgCard.Id,
            Name = tcgCard.Name,
            Number = tcgCard.Number,
            SetId = tcgCard.SetId,
            SetName = tcgCard.SetName,
            Rarity = tcgCard.Rarity,
            ImageUrl = tcgCard.ImageSmall,
            ImageUrlLarge = tcgCard.ImageLarge,
            Variant = request.Variant,
            Quantity = 1,
            CreatedAt = DateTime.UtcNow
        };
 
        await userCardRepository.AddAsync(card);
        await unitOfWork.SaveChangesAsync();
 
        return MapToResponse(card);
    }
 
    private static CardResponse MapToResponse(UserCard card) => new()
    {
        Id = card.Id,
        TcgCardId = card.TcgCardId,
        Name = card.Name,
        Number = card.Number,
        SetId = card.SetId,
        SetName = card.SetName,
        Rarity = card.Rarity,
        ImageSmall = card.ImageUrl,
        ImageLarge = card.ImageUrlLarge,
        Variant = card.Variant,
        Quantity = card.Quantity
    };
}