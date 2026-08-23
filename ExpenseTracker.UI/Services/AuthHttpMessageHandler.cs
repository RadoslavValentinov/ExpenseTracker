using System.Net.Http.Headers;

namespace ExpenseTracker.UI.Services;

public class AuthHttpMessageHandler : DelegatingHandler
{
    private readonly AuthStateService _authState;

    public AuthHttpMessageHandler(
        AuthStateService authState)
    {
        _authState = authState;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token =
            await _authState.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }

        return await base.SendAsync(
            request,
            cancellationToken);
    }
}