using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Exceptions;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Infrastructure.Data;

namespace MyTCGBinder.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly Context _db;

    public UserRepository(Context db)
    {
        _db = db;
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException("Usuário não encontrado");
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _db.Users.AnyAsync(u => u.Email == email);
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
    }

    public Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid userId)
    {
        var user = await _db.Users.Where(a => a.Id == userId).FirstOrDefaultAsync() ?? throw new NotFoundException("Usuário não encontrado");
        _db.Users.Remove(user);
    }
}