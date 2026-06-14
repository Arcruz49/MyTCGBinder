using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
 
namespace MyTCGBinder.Application.UseCases;
 
public class GetCollectionUseCase(IUserCardRepository userCardRepository) : IGetCollectionUseCase
{
    public async Task<PagedResponse<CardResponse>> ExecuteAsync(Guid userId, int page, int pageSize, string? search)
    {
        var cards = await userCardRepository.GetAllByUserIdAsync(userId);
 
        if (!string.IsNullOrWhiteSpace(search))
            cards = cards.Where(c => c.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
 
        var total = cards.Count();
        var items = cards
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToResponse);
 
        return new PagedResponse<CardResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
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