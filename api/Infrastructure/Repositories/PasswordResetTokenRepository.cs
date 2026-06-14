using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Infrastructure.Data;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Domain.Exceptions;

namespace MyTCGBinder.Infrastructure.Repositories;

public class PasswordResetTokenRepository(Context db) : BaseRepository<PasswordResetToken>(db), IPasswordResetTokenRepository
{
    
    public async Task<PasswordResetToken?> GetByToken(string token)
    {
        return await Query().Where(a => a.Token == token).FirstOrDefaultAsync();
    }
    public async Task AddToken(PasswordResetToken passwordResetToken)
    {
        await AddAsync(passwordResetToken);
    }
    public async Task UpdateToken(PasswordResetToken passwordResetToken)
    {
        Update(passwordResetToken);
    }

}