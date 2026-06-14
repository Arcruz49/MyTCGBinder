using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTCGBinder.Application.Interfaces;

namespace MyTCGBinder.Controllers;

[ApiController]
[Route("tcg")]
public class TcgController : BaseController
{
    private readonly ISearchCardsUseCase _searchCardsUseCase;
    private readonly IGetSetsUseCase _getSetsUseCase;

    public TcgController(
        ISearchCardsUseCase searchCardsUseCase,
        IGetSetsUseCase getSetsUseCase)
    {
        _searchCardsUseCase = searchCardsUseCase;
        _getSetsUseCase = getSetsUseCase;
    }

    [HttpGet("sets")]
    public async Task<IActionResult> GetSets()
    {
        var result = await _getSetsUseCase.ExecuteAsync();
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? name, [FromQuery] string? setId)
    {
        var result = await _searchCardsUseCase.ExecuteAsync(name, setId);
        return Ok(result);
    }
}