using MyTCGBinder.Domain.Entities;

namespace MyTCGBinder.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<bool> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    void UpdateAsync(User user);
    void DeleteAsync(User user);
}