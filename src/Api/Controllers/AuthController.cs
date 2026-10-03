using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TMS.Identity.Application.DTOs;
using TMS.Identity.Application.Interfaces;

namespace TMS.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    [EndpointName("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var result = await _auth.RegisterAsync(request);
        if (!result.Succeeded)
        {
            var errors = new Dictionary<string, string[]> { ["register"] = [.. result.Errors] };
            return BadRequest(new ValidationProblemDetails(errors));
        }

        return Ok(result.Response);
    }

    [HttpPost("login")]
    [EndpointName("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _auth.LoginAsync(request);
        if (!result.Succeeded)
            return Unauthorized(new ProblemDetails { Status = 401, Detail = result.Errors[0] });

        return Ok(result.Response);
    }
}