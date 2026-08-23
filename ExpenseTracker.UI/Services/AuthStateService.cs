using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace ExpenseTracker.UI.Services;

public class AuthStateService
{
    private const string TokenKey = "expenseTracker.token";

    private readonly ProtectedLocalStorage _storage;

    private string? _token;
    private bool _initialized;

    public AuthStateService(
        ProtectedLocalStorage storage)
    {
        _storage = storage;
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
            var result =
                await _storage.GetAsync<string>(TokenKey);

            if (result.Success &&
                !string.IsNullOrWhiteSpace(result.Value))
            {
                _token = result.Value;
            }
            else
            {
                _token = null;
            }
        }
        catch
        {
            _token = null;
        }

        _initialized = true;
    }

    public async Task<string?> GetTokenAsync()
    {
        await InitializeAsync();

        return _token;
    }

    public async Task SetTokenAsync(
        string token)
    {
        _token = token;

        await _storage.SetAsync(
            TokenKey,
            token);

        _initialized = true;
    }

    public async Task LogoutAsync()
    {
        _token = null;

        await _storage.DeleteAsync(
            TokenKey);

        _initialized = true;
    }
}