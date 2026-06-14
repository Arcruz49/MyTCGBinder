using Microsoft.AspNetCore.Identity;
using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Application.Security;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Domain.ValueObjects;

namespace MyTCGBinder.Application.UseCases;

public class RegisterUserUseCase : IRegisterUserUseCase{

    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;


    public RegisterUserUseCase(IUserRepository userRepository, JwtTokenGenerator jwtTokenGenerator, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = new PasswordHasher<User>();
        _tokenGenerator = jwtTokenGenerator;
        _unitOfWork = unitOfWork;
    }
    public async Task<UserDto> ExecuteAsync(RegisterUserRequest request)
    {
        var email = new Email(request.Email);

        if (await _userRepository.GetByEmailAsync(email.Value) != null)
            throw new ValidationException("Email já cadastrado.");        
        
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = email.Value,
            CreatedAt = DateTime.UtcNow,
        };


        var password = new Password(request.Password);
        user.Password = _passwordHasher.HashPassword(user, password.Value);    
        

        await _userRepository.AddAsync(user);

        await _unitOfWork.SaveChangesAsync();

        var token = _tokenGenerator.GenerateToken(user.Id, user.Name);

        return new UserDto(user.Name, user.Email, token);
    }
}
