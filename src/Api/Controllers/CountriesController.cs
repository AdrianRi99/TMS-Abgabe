using Microsoft.AspNetCore.Mvc;
using TMS.Addresses.Application.DTOs.CountryDTO;
using TMS.Addresses.Application.Handlers;

namespace TMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CountriesController : ControllerBase
{
    private readonly CountryHandlers _handlers;

    public CountriesController(CountryHandlers handlers)
    {
        _handlers = handlers;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> GetAll(CancellationToken ct)
    {
        var result = await _handlers.GetAllAsync(ct);
        return Ok(result);
    }
}