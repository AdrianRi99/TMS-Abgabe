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

    public AuthService(UserManager<ApplicationUser> users, JwtTokenService tokens)
    {
        _users = users;
        _tokens = tokens;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };

        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return AuthResult.Failure(result.Errors.Select(ToMessage).Distinct().ToArray());

        return AuthResult.Success(CreateResponse(user));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request)
    {
        var user = await _users.FindByEmailAsync(request.Email);
        if (user is null)
            return AuthResult.Failure(InvalidCredentials);

        if (await _users.IsLockedOutAsync(user))
            return AuthResult.Failure(LockedOut);

        if (!await _users.CheckPasswordAsync(user, request.Password))
        {
            await _users.AccessFailedAsync(user);
            return AuthResult.Failure(InvalidCredentials);
        }

        await _users.ResetAccessFailedCountAsync(user);
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