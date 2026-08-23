namespace ExpenseTracker.UI.Services;

public class AuthStateService
{
    private string? _token;

    public bool IsAuthenticated =>
        !string.IsNullOrWhiteSpace(_token);

    public string? Token =>
        _token;


    public Task<string?> GetTokenAsync()
    {
        return Task.FromResult(_token);
    }


    public Task SetTokenAsync(string token)
    {
        _token = token;

        return Task.CompletedTask;
    }


    public Task LogoutAsync()
    {
        _token = null;

        return Task.CompletedTask;
    }
}