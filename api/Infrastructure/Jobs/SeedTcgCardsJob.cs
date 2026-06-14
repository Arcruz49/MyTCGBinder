using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MyTCGBinder.Domain.Entities;
using MyTCGBinder.Infrastructure.Data;

namespace MyTCGBinder.Infrastructure.Jobs;

public class SeedTcgCardsJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<SeedTcgCardsJob> _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private Dictionary<string, string> _setSeriesMap = [];

    public SeedTcgCardsJob(
        IServiceScopeFactory scopeFactory,
        IHttpClientFactory httpClientFactory,
        ILogger<SeedTcgCardsJob> logger)
    {
        _scopeFactory = scopeFactory;
        _httpClient = httpClientFactory.CreateClient("tcg");
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Context>();

        var existingCount = await db.TCGCards.CountAsync(stoppingToken);
        if (existingCount > 20000)
        {
            _logger.LogInformation("TCG cards já estão no banco ({Count}). Seed ignorado.", existingCount);
            return;
        }

        if (existingCount > 0)
            _logger.LogInformation("Seed incompleto detectado ({Count} cartas). Continuando...", existingCount);
        else
            _logger.LogInformation("Iniciando seed das cartas TCG...");

        await SeedSetsAsync(stoppingToken);
        await SeedCardsAsync(db, stoppingToken);

        _logger.LogInformation("Seed concluído.");
    }

    private async Task SeedSetsAsync(CancellationToken ct)
    {
        for (var attempt = 1; attempt <= 3; attempt++)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<TcgApiListResponse<TcgApiSet>>(
                    "sets?select=id,name,series&orderBy=releaseDate",
                    _jsonOptions,
                    ct
                );

                _setSeriesMap = response?.Data?
                    .ToDictionary(s => s.Id, s => s.Series ?? string.Empty)
                    ?? [];

                _logger.LogInformation("Sets carregados: {Count}", _setSeriesMap.Count);
                return;
            }
            catch (Exception ex) when (attempt < 3)
            {
                _logger.LogWarning("Sets — tentativa {Attempt}/3 falhou: {Message}. Aguardando {Delay}s...",
                    attempt, ex.Message, attempt * 5);
                await Task.Delay(TimeSpan.FromSeconds(attempt * 5), ct);
            }
        }

        _logger.LogError("Não foi possível carregar os sets após 3 tentativas. Continuando sem series.");
    }

    private async Task SeedCardsAsync(Context db, CancellationToken ct)
    {
        const int pageSize = 250;
        var syncedAt = DateTime.UtcNow;

        var existingCount = await db.TCGCards.CountAsync(ct);
        var page = existingCount > 0 ? (existingCount / pageSize) + 1 : 1;
        var total = existingCount;

        if (existingCount > 0)
            _logger.LogInformation("Continuando da página {Page} ({Total} cartas já salvas)", page, total);

        while (!ct.IsCancellationRequested)
        {
            TcgApiPagedResponse<TcgApiCard>? response = null;

            for (var attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    response = await _httpClient.GetFromJsonAsync<TcgApiPagedResponse<TcgApiCard>>(
                        $"cards?page={page}&pageSize={pageSize}&select=id,name,number,set,rarity,images",
                        _jsonOptions,
                        ct
                    );
                    break;
                }
                catch (Exception ex) when (attempt < 3)
                {
                    _logger.LogWarning("Página {Page} — tentativa {Attempt}/3 falhou: {Message}. Aguardando {Delay}s...",
                        page, attempt, ex.Message, attempt * 5);
                    await Task.Delay(TimeSpan.FromSeconds(attempt * 5), ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Página {Page} — todas as tentativas falharam: {Message}. Abortando seed.", page, ex.Message);
                    return;
                }
            }

            if (response?.Data is null || response.Data.Count == 0)
                break;

            var batch = response.Data.Select(card => new TCGCard
            {
                Id = card.Id,
                Name = card.Name,
                Number = card.Number,
                SetId = card.Set?.Id ?? string.Empty,
                SetName = card.Set?.Name ?? string.Empty,
                Series = card.Set?.Id is not null && _setSeriesMap.TryGetValue(card.Set.Id, out var series)
                    ? series : string.Empty,
                Rarity = card.Rarity ?? string.Empty,
                ImageSmall = card.Images?.Small ?? string.Empty,
                ImageLarge = card.Images?.Large ?? string.Empty,
                SyncedAt = syncedAt
            }).ToList();

            await db.TCGCards.AddRangeAsync(batch, ct);
            await db.SaveChangesAsync(ct);

            total += batch.Count;
            _logger.LogInformation("Página {Page} salva — {Total} cartas no total", page, total);

            if (response.Data.Count < pageSize)
                break;

            page++;
            await Task.Delay(500, ct);
        }
    }

    private class TcgApiPagedResponse<T>
    {
        public List<T>? Data { get; set; }
        public int TotalCount { get; set; }
    }

    private class TcgApiListResponse<T>
    {
        public List<T>? Data { get; set; }
    }

    private class TcgApiCard
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string? Rarity { get; set; }
        public TcgApiSet? Set { get; set; }
        public TcgApiImages? Images { get; set; }
    }

    private class TcgApiSet
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Series { get; set; }
    }

    private class TcgApiImages
    {
        public string? Small { get; set; }
        public string? Large { get; set; }
    }
}