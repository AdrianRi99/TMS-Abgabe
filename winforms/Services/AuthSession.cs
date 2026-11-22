using System.Net.Http.Json;

namespace TMS.WinForms.Services;

public sealed class AuthSession
{
    private const string DemoEmail = "demo@tms.dev";
    private const string DemoPassword = "Demo1234";

    public string? Token { get; private set; }

    public async Task<bool> LoginAsDemoAsync(HttpClient httpClient)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", new
        {
            email = DemoEmail,
            password = DemoPassword
        });

        if (!response.IsSuccessStatusCode) return false;

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Token = result?.Token;
        return Token is not null;
    }

    private record LoginResponse(string Token, DateTime ExpiresAtUtc, string Email);
}