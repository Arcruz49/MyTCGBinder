using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace MyTCGBinder.Application.UseCases;

public class SendEmailForgotPasswordUseCase(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IUnitOfWork unitOfWork,
    IEmailService emailService
    ) : ISendEmailForgotPasswordUseCase{

    public async Task ExecuteAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email é obrigatório.");

        var user = await userRepository.GetByEmailAsync(email);

        if(user == null) return;

        var rawToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        var hashedToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(rawToken)));

        var passwordToken = new PasswordResetToken()
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = hashedToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            CreatedAt = DateTime.UtcNow,
        };

        await passwordResetTokenRepository.AddToken(passwordToken);

        await unitOfWork.SaveChangesAsync();
        //envia email
        await emailService.SendPasswordResetAsync(user.Email, rawToken);
    }

}
