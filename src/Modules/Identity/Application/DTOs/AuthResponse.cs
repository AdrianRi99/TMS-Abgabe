namespace TMS.Identity.Application.DTOs;

public record AuthResponse(string Token, DateTime ExpiresAtUtc, string Email);