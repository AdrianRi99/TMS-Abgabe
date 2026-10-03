using Microsoft.AspNetCore.Mvc;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Handlers;
using TMS.Addresses.Application.Common;

namespace TMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly AddressHandlers _handlers;

    public AddressesController(AddressHandlers handlers)
    {
        _handlers = handlers;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<AddressDto>> GetById(
        Guid id,
        CancellationToken ct)
    {
        var result = await _handlers.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<AddressDto>>> Search(
        [FromQuery] AddressSearchRequest request,
        CancellationToken ct)
    {
        var result = await _handlers.SearchAsync(request, ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AddressDto>> Create(
        CreateAddressRequest request,
        CancellationToken ct)
    {
        var result = await _handlers.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AddressDto>> Update(
        Guid id,
        UpdateAddressRequest request,
        CancellationToken ct)
    {
        var result = await _handlers.UpdateAsync(id, request, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _handlers.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("suggestions/streets")]
    public async Task<ActionResult<IReadOnlyList<string>>> StreetSuggestions(
    [FromQuery] string term = "",
    CancellationToken ct = default)
    {
        var result = await _handlers.GetStreetSuggestionsAsync(term, ct);
        return Ok(result);
    }

    [HttpGet("suggestions/cities")]
    public async Task<ActionResult<IReadOnlyList<string>>> CitySuggestions(
        [FromQuery] string term = "",
        CancellationToken ct = default)
    {
        var result = await _handlers.GetCitySuggestionsAsync(term, ct);
        return Ok(result);
    }

    [HttpGet("suggestions/countries")]
    public async Task<ActionResult<IReadOnlyList<string>>> CountrySuggestions(
        [FromQuery] string term = "",
        CancellationToken ct = default)
    {
        var result = await _handlers.GetCountrySuggestionsAsync(term, ct);
        return Ok(result);
    }
}