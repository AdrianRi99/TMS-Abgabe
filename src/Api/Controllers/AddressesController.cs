using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.Addresses.Application.DTOs.AddressDTO;
using TMS.Addresses.Application.Common;
using TMS.Addresses.Application.Handlers;

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
    [EndpointName("getAddressById")]
    public async Task<ActionResult<AddressDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _handlers.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [EndpointName("searchAddresses")]
    public async Task<ActionResult<PagedResult<AddressDto>>> Search(
        [FromQuery] string? street,
        [FromQuery] string? cityName,
        [FromQuery] string? countryName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var request = new AddressSearchRequest(street, cityName, countryName, page, pageSize);
        var result = await _handlers.SearchAsync(request, ct);
        return Ok(result);
    }

    [HttpPost]
    [EndpointName("createAddress")]
    public async Task<ActionResult<AddressDto>> Create(
        CreateAddressRequest request, CancellationToken ct)
    {
        var result = await _handlers.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [EndpointName("updateAddress")]
    public async Task<ActionResult<AddressDto>> Update(
        Guid id, UpdateAddressRequest request, CancellationToken ct)
    {
        var result = await _handlers.UpdateAsync(id, request, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [EndpointName("deleteAddress")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _handlers.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}