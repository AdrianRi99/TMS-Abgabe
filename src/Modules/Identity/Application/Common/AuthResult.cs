using TMS.Identity.Application.DTOs;

namespace TMS.Identity.Application.Common;

public sealed record AuthResult(AuthResponse? Response, IReadOnlyList<string> Errors)
{
    public bool Succeeded => Response is not null;

    public static AuthResult Success(AuthResponse response) => new(response, []);

    public static AuthResult Failure(params string[] errors) => new(null, errors);
}