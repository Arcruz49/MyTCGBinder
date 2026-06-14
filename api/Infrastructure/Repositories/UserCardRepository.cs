using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Enums;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Infrastructure.Data;

namespace MyTCGBinder.Infrastructure.Repositories;

public class UserCardRepository : IUserCardRepository
{
    private readonly Context _db;

    public UserCardRepository(Context db)
    {
        _db = db;
    }

    public async Task<UserCard?> GetByIdAsync(Guid id)
    {
        return await _db.UserCards
            .AsNoTracking()
            .FirstOrDefaultAsync(uc => uc.Id == id)
            ?? throw new NotFoundException("Carta não encontrada");
    }

    public async Task<UserCard?> GetByTcgCardIdAndVariantAsync(Guid userId, string tcgCardId, CardVariant variant)
    {
        return await _db.UserCards
            .AsNoTracking()
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.TcgCardId == tcgCardId && uc.Variant == variant);
    }

    public async Task<IEnumerable<UserCard>> GetAllByUserIdAsync(Guid userId)
    {
        return await _db.UserCards
            .AsNoTracking()
            .Where(uc => uc.UserId == userId)
            .OrderBy(uc => uc.SetId)
            .ThenBy(uc => uc.Number)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountByUserIdAsync(Guid userId)
    {
        return await _db.UserCards
            .Where(uc => uc.UserId == userId)
            .SumAsync(uc => uc.Quantity);
    }

    public async Task AddAsync(UserCard card)
    {
        await _db.UserCards.AddAsync(card);
    }

    public Task UpdateAsync(UserCard card)
    {
        _db.UserCards.Update(card);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(UserCard card)
    {
        _db.UserCards.Remove(card);
        return Task.CompletedTask;
    }
}