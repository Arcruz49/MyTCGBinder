using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyTCGBinder.Application.DTOs.Request;
using MyTCGBinder.Application.Interfaces;

namespace MyTCGBinder.Controllers;

[Authorize]
[ApiController]
[Route("cards")]
public class CardController : BaseController
{
    private readonly IGetCollectionUseCase _getCollectionUseCase;
    private readonly IGetCollectionCountUseCase _getCollectionCountUseCase;
    private readonly IAddCardUseCase _addCardUseCase;
    private readonly IUpdateCardQuantityUseCase _updateCardQuantityUseCase;
    private readonly IDeleteCardUseCase _deleteCardUseCase;

    public CardController(
        IGetCollectionUseCase getCollectionUseCase,
        IGetCollectionCountUseCase getCollectionCountUseCase,
        IAddCardUseCase addCardUseCase,
        IUpdateCardQuantityUseCase updateCardQuantityUseCase,
        IDeleteCardUseCase deleteCardUseCase)
    {
        _getCollectionUseCase = getCollectionUseCase;
        _getCollectionCountUseCase = getCollectionCountUseCase;
        _addCardUseCase = addCardUseCase;
        _updateCardQuantityUseCase = updateCardQuantityUseCase;
        _deleteCardUseCase = deleteCardUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetCollection(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null)
    {
        var result = await _getCollectionUseCase.ExecuteAsync(UserId, page, pageSize, search);
        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<IActionResult> GetCount()
    {
        var result = await _getCollectionCountUseCase.ExecuteAsync(UserId);
        return Ok(new { total = result });
    }

    [HttpPost]
    public async Task<IActionResult> AddCard([FromBody] AddCardRequest request)
    {
        var result = await _addCardUseCase.ExecuteAsync(UserId, request);
        return CreatedAtAction(nameof(GetCollection), result);
    }

    [HttpPatch("{id}/quantity")]
    public async Task<IActionResult> UpdateQuantity(Guid id, [FromBody] UpdateCardQuantityRequest request)
    {
        await _updateCardQuantityUseCase.ExecuteAsync(UserId, id, request);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCard(Guid id)
    {
        await _deleteCardUseCase.ExecuteAsync(UserId, id);
        return NoContent();
    }
}