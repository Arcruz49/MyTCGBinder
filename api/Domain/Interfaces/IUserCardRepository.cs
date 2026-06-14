using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Enums;

namespace MyTCGBinder.Domain.Interfaces;

public interface IUserCardRepository
{
    Task<UserCard> GetByIdAsync(Guid id);
    Task<UserCard?> GetByTcgCardIdAndVariantAsync(Guid userId, string tcgCardId, CardVariant variant);
    Task<IEnumerable<UserCard>> GetAllByUserIdAsync(Guid userId);
    Task<int> GetTotalCountByUserIdAsync(Guid userId);
    Task AddAsync(UserCard card);
    void UpdateAsync(UserCard card);
    void DeleteAsync(UserCard card);
}