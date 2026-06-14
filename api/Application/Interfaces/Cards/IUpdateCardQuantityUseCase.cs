using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface IUpdateCardQuantityUseCase
{
    Task<CardResponse> ExecuteAsync(Guid userId, Guid cardId, UpdateCardQuantityRequest request);
}