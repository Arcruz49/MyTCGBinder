using System.Net.Http.Json;
using System.Text.Json;
using MyTCGBinder.Application.DTOs.Responses;
using MyTCGBinder.Application.Interfaces;
using MyTCGBinder.Infrastructure.TcgApi.Models;

namespace MyTCGBinder.Infrastructure.TcgApi;

public class TcgService(HttpClient httpClient) : ITcgService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IEnumerable<TcgCardResponse>> SearchAsync(string? name, string? setId)
    {
        var query = BuildSearchQuery(name, setId);

        var response = await httpClient.GetFromJsonAsync<TcgApiListResponse<TcgApiCard>>(
            $"cards?q={Uri.EscapeDataString(query)}&select=id,name,number,set,rarity,images",
            _jsonOptions
        );

        return response?.Data?.Select(MapCardToResponse) ?? [];
    }

    public async Task<TcgCardResponse?> GetByIdAsync(string tcgCardId)
    {
        var response = await httpClient.GetFromJsonAsync<TcgApiSingleResponse<TcgApiCard>>(
            $"cards/{tcgCardId}",
            _jsonOptions
        );

        return response?.Data is not null ? MapCardToResponse(response.Data) : null;
    }

    public async Task<IEnumerable<TcgSetResponse>> GetSetsAsync()
    {
        var response = await httpClient.GetFromJsonAsync<TcgApiListResponse<TcgApiSet>>(
            "sets?select=id,name,series,images&orderBy=releaseDate",
            _jsonOptions
        );

        return response?.Data?.Select(MapSetToResponse) ?? [];
    }

    private static string BuildSearchQuery(string? name, string? setId)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(name))
            parts.Add($"name:{name}*");

        if (!string.IsNullOrWhiteSpace(setId))
            parts.Add($"set.id:{setId}");

        return string.Join(" ", parts);
    }

    private static TcgCardResponse MapCardToResponse(TcgApiCard card) => new()
    {
        Id = card.Id,
        Name = card.Name,
        Number = card.Number,
        SetId = card.Set?.Id ?? string.Empty,
        SetName = card.Set?.Name ?? string.Empty,
        Rarity = card.Rarity ?? string.Empty,
        ImageSmall = card.Images?.Small ?? string.Empty,
        ImageLarge = card.Images?.Large ?? string.Empty
    };

    private static TcgSetResponse MapSetToResponse(TcgApiSet set) => new()
    {
        Id = set.Id,
        Name = set.Name,
        Series = set.Series ?? string.Empty,
        Logo = set.Images?.Logo ?? string.Empty
    };
}