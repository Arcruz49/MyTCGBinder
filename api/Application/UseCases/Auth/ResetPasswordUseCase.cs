using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using MyTCGBinder.Domain.ValueObjects;

namespace MyTCGBinder.Application.UseCases;

public class ResetPasswordUseCase(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IUnitOfWork unitOfWork,
    PasswordHasher<User> passwordHasher
    ) : IResetPasswordUseCase{

    public async Task ExecuteAsync(string token, string password)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ValidationException("Token inválido.");

        if (string.IsNullOrWhiteSpace(password))
            throw new ValidationException("Senha é obrigatória.");

        var hashedToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        var passwordToken = await passwordResetTokenRepository.GetByToken(hashedToken)
            ?? throw new ValidationException("Token inválido.");

        if (passwordToken.ExpiresAt < DateTime.UtcNow)
            throw new ValidationException("Token expirado.");

        if (passwordToken.UsedAt is not null)
            throw new ValidationException("Token já utilizado.");

        var user = await userRepository.GetByIdAsync(passwordToken.UserId);

        var passwordObject = new Password(password);
        user.Password = passwordHasher.HashPassword(user, passwordObject.Value);

        passwordToken.UsedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user);
        
        await passwordResetTokenRepository.UpdateToken(passwordToken);

        await unitOfWork.SaveChangesAsync();
    }

}
