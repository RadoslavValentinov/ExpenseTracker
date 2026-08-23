using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class AuthService
{
    private readonly IHttpClientFactory _factory;

    public AuthService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<LoginResponse?> LoginAsync(
        string email,
        string password)
    {
        // IMPORTANT:
        // Login must NOT use the authenticated API client,
        // because there is no JWT yet.

        var client = _factory.CreateClient("AuthApi");

        var response = await client.PostAsJsonAsync(
            "api/Auth/login",
            new
            {
                email,
                password
            });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<LoginResponse>();
    }
}


public class LoginResponse
{
    public string Message { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}