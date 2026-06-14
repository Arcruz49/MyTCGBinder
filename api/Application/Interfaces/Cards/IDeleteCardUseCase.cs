using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface IDeleteCardUseCase
{
    Task ExecuteAsync(Guid userId, Guid cardId);
}
