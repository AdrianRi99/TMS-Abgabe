using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using TMS.Identity.Application.Common;
using TMS.Identity.Application.DTOs;
using TMS.Identity.Application.Interfaces;
using TMS.Identity.Infrastructure.Persistence;

namespace TMS.Identity.Infrastructure.Authentication;

public sealed class AuthService : IAuthService
{
    private const string InvalidCredentials = "E-Mail oder Passwort ist falsch.";
    private const string LockedOut = "Zu viele Fehlversuche. Bitte später erneut versuchen.";

    private readonly UserManager<ApplicationUser> _users;
    private readonly JwtTokenService _tokens;
    private readonly ILogger<AuthService> _logger;


    public AuthService(UserManager<ApplicationUser> users, JwtTokenService tokens, ILogger<AuthService> logger)
    {
        _users = users;
        _tokens = tokens;
        _logger = logger;
    }


    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for {Email}", request.Email);

        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
        var result = await _users.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Registration failed for {Email}: {Errors}",
                request.Email,
                string.Join(", ", result.Errors.Select(e => e.Code)));
            return AuthResult.Failure(result.Errors.Select(ToMessage).Distinct().ToArray());
        }

        _logger.LogInformation("User registered: {Email}", request.Email);
        return AuthResult.Success(CreateResponse(user));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        _logger.LogDebug("Login attempt for {Email}", request.Email);

        var user = await _users.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogWarning("Login failed: unknown email {Email}", request.Email);
            return AuthResult.Failure(InvalidCredentials);
        }

        if (await _users.IsLockedOutAsync(user))
        {
            _logger.LogWarning("Login blocked: user {Email} is locked out", request.Email);
            return AuthResult.Failure(LockedOut);
        }

        if (!await _users.CheckPasswordAsync(user, request.Password))
        {
            await _users.AccessFailedAsync(user);
            _logger.LogWarning(
                "Login failed: wrong password for {Email} (failed attempts: {Count})",
                request.Email,
                await _users.GetAccessFailedCountAsync(user));
            return AuthResult.Failure(InvalidCredentials);
        }

        await _users.ResetAccessFailedCountAsync(user);
        _logger.LogInformation("User logged in: {Email}", request.Email);
        return AuthResult.Success(CreateResponse(user));
    }

    private AuthResponse CreateResponse(ApplicationUser user)
    {
        var (token, expires) = _tokens.Create(user);
        return new AuthResponse(token, expires, user.Email ?? string.Empty);
    }

    private static string ToMessage(IdentityError error) => error.Code switch
    {
        "DuplicateUserName" or "DuplicateEmail" => "Diese E-Mail-Adresse ist bereits registriert.",
        "InvalidEmail" or "InvalidUserName" => "Ungültige E-Mail-Adresse.",
        "PasswordTooShort" => "Das Passwort ist zu kurz.",
        "PasswordRequiresDigit" => "Das Passwort muss mindestens eine Ziffer enthalten.",
        "PasswordRequiresLower" => "Das Passwort muss mindestens einen Kleinbuchstaben enthalten.",
        _ => "Registrierung fehlgeschlagen."
    };
}