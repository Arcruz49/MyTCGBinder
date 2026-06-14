using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface IGetCollectionUseCase
{
    Task<PagedResponse<CardResponse>> ExecuteAsync(Guid userId, int page, int pageSize, string? search);
}









