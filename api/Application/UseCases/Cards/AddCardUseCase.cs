using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;

namespace MyTCGBinder.Application.UseCases;
public class AddCardUseCase(
    IUserCardRepository userCardRepository,
    ITCGCardRepository tcgCardRepository,
    IUnitOfWork unitOfWork) : IAddCardUseCase
{
    public async Task<CardResponse> ExecuteAsync(Guid userId, AddCardRequest request)
    {
        var tcgCard = await tcgCardRepository.GetByIdAsync(request.TcgCardId)
            ?? throw new NotFoundException("Carta não encontrada");

        var existing = await userCardRepository.GetByTcgCardIdAndVariantAsync(userId, request.TcgCardId, request.Variant);

        if (existing is not null)
        {
            existing.Quantity++;
            userCardRepository.UpdateAsync(existing);
            await unitOfWork.SaveChangesAsync();

            return MapToResponse(existing, tcgCard);
        }

        var card = new UserCard
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TcgCardId = tcgCard.Id,
            Variant = request.Variant,
            Quantity = 1,
            CreatedAt = DateTime.UtcNow
        };

        await userCardRepository.AddAsync(card);
        await unitOfWork.SaveChangesAsync();

        return MapToResponse(card, tcgCard);
    }

    private static CardResponse MapToResponse(UserCard card, TCGCard tcgCard) => new()
    {
        Id = card.Id,
        TcgCardId = card.TcgCardId,
        Name = tcgCard.Name,
        Number = tcgCard.Number,
        SetId = tcgCard.SetId,
        SetName = tcgCard.SetName,
        Rarity = tcgCard.Rarity,
        ImageSmall = tcgCard.ImageSmall,
        ImageLarge = tcgCard.ImageLarge,
        Variant = card.Variant,
        Quantity = card.Quantity
    };
}
