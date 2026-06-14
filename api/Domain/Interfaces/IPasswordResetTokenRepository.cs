using MyTCGBinder.Domain.Entities;

namespace MyTCGBinder.Domain.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> GetByToken(string token);
    Task AddToken(PasswordResetToken passwordResetToken);
    Task UpdateToken(PasswordResetToken passwordResetToken);
}
