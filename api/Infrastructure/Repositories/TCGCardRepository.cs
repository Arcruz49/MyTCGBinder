using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Domain.Interfaces;
using MyTCGBinder.Infrastructure.Data;

namespace MyTCGBinder.Infrastructure.Repositories;

public class TCGCardRepository(Context db) : BaseRepository<TCGCard>(db), ITCGCardRepository
{
    public async Task<TCGCard?> GetByIdAsync(string id)
    {
        return await Query().FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<TCGCard>> SearchAsync(string? name, string? setId)
    {
        var query = Query();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(c => c.Name.ToLower().Contains(name.ToLower()));

        if (!string.IsNullOrWhiteSpace(setId))
            query = query.Where(c => c.SetId == setId);

        return await query
            .OrderBy(c => c.SetId)
            .ThenBy(c => c.Number)
            .Take(100)
            .ToListAsync();
    }

    public async Task<IEnumerable<TCGCard>> GetAllSetsAsync()
    {
        return await Query()
            .GroupBy(c => new { c.SetId, c.SetName, c.Series })
            .Select(g => new TCGCard
            {
                Id = g.Key.SetId,
                SetId = g.Key.SetId,
                SetName = g.Key.SetName,
                Series = g.Key.Series
            })
            .OrderBy(c => c.Series)
            .ThenBy(c => c.SetName)
            .ToListAsync();
    }
}