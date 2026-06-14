using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Infrastructure.Data;

namespace MyTCGBinder.Infrastructure.Repositories;

public class UserRepository(Context db) : BaseRepository<User>(db), IUserRepository
{
    public async Task<User> GetByIdAsync(Guid id)
    {
        return await FindAsync(id)
            ?? throw new NotFoundException("Usuário não encontrado");
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await Query().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await Query().AnyAsync(u => u.Email == email);
    }

    public new async Task AddAsync(User user) => await base.AddAsync(user);
    public void UpdateAsync(User user) => Update(user);

    public void DeleteAsync(User user) => Remove(user);
}