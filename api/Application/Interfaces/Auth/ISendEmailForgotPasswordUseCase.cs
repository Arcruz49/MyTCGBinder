using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface ISendEmailForgotPasswordUseCase
{
    Task ExecuteAsync(string email);
}