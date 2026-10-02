using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;

namespace ExpenseTracker.UI.Services;

public class AuthStateService
{
    private const string TokenKey =
        "expenseTracker.token";


    private readonly IJSRuntime _js;


    private string? _token;

    private bool _initialized;


    public AuthStateService(IJSRuntime js)
    {
        _js = js;
    }


    // =========================
    // AUTHENTICATION STATE
    // =========================

    public bool IsAuthenticated
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_token))
                return false;


            try
            {
                var handler =
                    new JwtSecurityTokenHandler();

                var jwt =
                    handler.ReadJwtToken(_token);


                return jwt.ValidTo > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }


    public string? Token =>
        _token;


    // =========================
    // INITIALIZATION
    // =========================

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


    // =========================
    // TOKEN
    // =========================

    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }


    public async Task SetTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            await LogoutAsync();
            return;
        }


        try
        {
            var handler =
                new JwtSecurityTokenHandler();

            var jwt =
                handler.ReadJwtToken(token);


            if (jwt.ValidTo <= DateTime.UtcNow)
            {
                await LogoutAsync();
                return;
            }
        }
        catch
        {
            await LogoutAsync();
            return;
        }


        _token =
            token;


        try
        {
            await _js.InvokeVoidAsync(
                "localStorage.setItem",
                TokenKey,
                token);
        }
        catch
        {
            _token = null;

            throw;
        }


        _initialized = true;
    }


    // =========================
    // LOGOUT
    // =========================

    public async Task LogoutAsync()
    {
        _token = null;

        _initialized = true;


        try
        {
            await _js.InvokeVoidAsync(
                "localStorage.removeItem",
                TokenKey);
        }
        catch
        {
            // The in-memory authentication state
            // is already cleared.
        }
    }
}