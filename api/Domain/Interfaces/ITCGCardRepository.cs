using MyTCGBinder.Domain.Entities;

namespace MyTCGBinder.Domain.Interfaces;

public interface ITCGCardRepository
{
    Task<TCGCard?> GetByIdAsync(string id);
    Task<IEnumerable<TCGCard>> SearchAsync(string? name, string? setId);
    Task<IEnumerable<TCGCard>> GetAllSetsAsync();
}