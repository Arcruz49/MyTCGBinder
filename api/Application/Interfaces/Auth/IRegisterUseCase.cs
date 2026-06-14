using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;

namespace MyTCGBinder.Application.Interfaces;

public interface IRegisterUserUseCase
{
    Task<UserDto> ExecuteAsync(RegisterUserRequest request);
}