using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface IResetPasswordUseCase
{
    Task ExecuteAsync(string token, string password);
}