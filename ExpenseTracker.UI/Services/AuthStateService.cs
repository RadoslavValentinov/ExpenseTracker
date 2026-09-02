using Microsoft.JSInterop;

namespace ExpenseTracker.UI.Services;

public class AuthStateService
{
    private const string TokenKey = "expenseTracker.token";

    private readonly IJSRuntime _js;

    private string? _token;
    private bool _initialized;

    public AuthStateService(IJSRuntime js)
    {
        _js = js;
    }

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(_token);

    public string? Token =>
        _token;


    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        try
        {
            _token =
                await _js.InvokeAsync<string?>(
                    "localStorage.getItem",
                    TokenKey);
        }
        catch
        {
            _token = null;
        }

        _initialized = true;
    }


    public async Task WaitForInitializationAsync()
    {
        await InitializeAsync();
    }


    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }


    public async Task SetTokenAsync(string token)
    {
        _token = token;

        await _js.InvokeVoidAsync(
            "localStorage.setItem",
            TokenKey,
            token);

        _initialized = true;
    }


    public async Task LogoutAsync()
    {
        _token = null;

        await _js.InvokeVoidAsync(
            "localStorage.removeItem",
            TokenKey);

        _initialized = true;
    }
}