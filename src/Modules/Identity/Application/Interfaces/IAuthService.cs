using TMS.Identity.Application.Common;
using TMS.Identity.Application.DTOs;

namespace TMS.Identity.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    Task<AuthResult> LoginAsync(LoginRequest request);
}