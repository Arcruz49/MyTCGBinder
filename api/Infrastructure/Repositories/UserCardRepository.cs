using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Enums;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Infrastructure.Data;

namespace MyTCGBinder.Infrastructure.Repositories;

public class UserCardRepository(Context db) : BaseRepository<UserCard>(db), IUserCardRepository
{
    public async Task<UserCard> GetByIdAsync(Guid id)
    {
        return await FindAsync(id)
            ?? throw new NotFoundException("Carta não encontrada");
    }

    public async Task<UserCard?> GetByTcgCardIdAndVariantAsync(Guid userId, string tcgCardId, CardVariant variant)
    {
        return await Query()
            .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.TcgCardId == tcgCardId && uc.Variant == variant);
    }

    public async Task<IEnumerable<UserCard>> GetAllByUserIdAsync(Guid userId)
    {
        return await Query()
            .Where(uc => uc.UserId == userId)
            .OrderBy(uc => uc.SetId)
            .ThenBy(uc => uc.Number)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountByUserIdAsync(Guid userId)
    {
        return await Query()
            .Where(uc => uc.UserId == userId)
            .SumAsync(uc => uc.Quantity);
    }

    public new async Task AddAsync(UserCard card) => await base.AddAsync(card);

    public void UpdateAsync(UserCard card) => Update(card);

    public void DeleteAsync(UserCard card) => Remove(card);
}