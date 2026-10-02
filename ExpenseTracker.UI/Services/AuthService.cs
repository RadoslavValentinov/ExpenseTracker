using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class AuthService
{
    private readonly IHttpClientFactory _factory;


    public AuthService(IHttpClientFactory factory)
    {
        _factory = factory;
    }


    // =========================
    // LOGIN
    // =========================

    public async Task<LoginResponse?> LoginAsync(
        string email,
        string password)
    {
        var client =
            _factory.CreateClient("AuthApi");


        using var response =
            await client.PostAsJsonAsync(
                "api/Auth/login",
                new
                {
                    email = email.Trim(),
                    password
                });


        if (!response.IsSuccessStatusCode)
            return null;


        var result =
            await response.Content
                .ReadFromJsonAsync<LoginResponse>();


        if (result == null)
            return null;


        if (string.IsNullOrWhiteSpace(result.Token))
            return null;


        return result;
    }
}


// =========================
// LOGIN RESPONSE
// =========================

public class LoginResponse
{
    public string Message { get; set; } =
        string.Empty;


    public string Token { get; set; } =
        string.Empty;


    public DateTime ExpiresAt { get; set; }
}