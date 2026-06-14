using MyTCGBinder.Application.DTOs.Request;

namespace MyTCGBinder.Application.Interfaces;

public interface IUpdateCardQuantityUseCase
{
    Task ExecuteAsync(Guid userId, Guid cardId, UpdateCardQuantityRequest request);
}