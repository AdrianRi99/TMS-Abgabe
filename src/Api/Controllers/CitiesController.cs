using Microsoft.AspNetCore.Mvc;
using TMS.Addresses.Application.DTOs.CityDTO;
using TMS.Addresses.Application.Handlers;

namespace TMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly CityHandlers _handlers;

    public CitiesController(CityHandlers handlers)
    {
        _handlers = handlers;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CityDto>>> GetAll(CancellationToken ct)
    {
        var result = await _handlers.GetAllAsync(ct);
        return Ok(result);
    }
}