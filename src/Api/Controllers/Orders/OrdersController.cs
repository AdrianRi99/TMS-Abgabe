using Microsoft.AspNetCore.Mvc;
using TMS.Orders.Application.Common;
using TMS.Orders.Application.DTOs;
using TMS.Orders.Application.Handlers;

namespace TMS.Api.Controllers.Orders;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderHandlers _handlers;

    public OrdersController(OrderHandlers handlers)
    {
        _handlers = handlers;
    }

    [HttpGet("{id:guid}")]
    [EndpointName("getOrderById")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _handlers.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    [EndpointName("searchOrders")]
    public async Task<ActionResult<PagedResult<OrderDto>>> Search(
        [FromQuery] OrderSearchRequest request, CancellationToken ct)
    {
        var result = await _handlers.SearchAsync(request, ct);
        return Ok(result);
    }

    [HttpPost]
    [EndpointName("createOrder")]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request, CancellationToken ct)
    {
        var result = await _handlers.CreateAsync(request, ct);
        if (result is null)
        {
            ModelState.AddModelError(nameof(CreateOrderRequest.AddressId), "Die Adresse existiert nicht.");
            return ValidationProblem(ModelState);
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [EndpointName("updateOrder")]
    public async Task<ActionResult<OrderDto>> Update(Guid id, UpdateOrderRequest request, CancellationToken ct)
    {
        var result = await _handlers.UpdateAsync(id, request, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [EndpointName("deleteOrder")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _handlers.DeleteAsync(id, ct);
        return deleted ? NoContent() : NotFound();
    }
}
