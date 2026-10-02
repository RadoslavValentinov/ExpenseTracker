using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace ExpenseTracker.UI.Services;

public class JwtAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly AuthStateService _authState;


    private static readonly ClaimsPrincipal Anonymous =
        new(new ClaimsIdentity());


    public JwtAuthenticationStateProvider(
        AuthStateService authState)
    {
        _authState = authState;
    }


    // =========================
    // AUTHENTICATION STATE
    // =========================

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        await _authState.InitializeAsync();


        var token =
            await _authState.GetTokenAsync();


        if (string.IsNullOrWhiteSpace(token))
        {
            return new AuthenticationState(
                Anonymous);
        }


        try
        {
            var handler =
                new JwtSecurityTokenHandler();


            var jwt =
                handler.ReadJwtToken(token);


            // =========================
            // TOKEN EXPIRATION
            // =========================

            if (jwt.ValidTo <= DateTime.UtcNow)
            {
                await _authState.LogoutAsync();


                return new AuthenticationState(
                    Anonymous);
            }


            // =========================
            // CLAIMS
            // =========================

            var identity =
                new ClaimsIdentity(
                    jwt.Claims,
                    "Bearer");


            var user =
                new ClaimsPrincipal(
                    identity);


            return new AuthenticationState(
                user);
        }
        catch
        {
            // Invalid or corrupted token.
            // Remove it so it cannot be reused.
            await _authState.LogoutAsync();


            return new AuthenticationState(
                Anonymous);
        }
    }


    // =========================
    // NOTIFY
    // =========================

    public void NotifyAuthenticationStateChanged()
    {
        NotifyAuthenticationStateChanged(
            GetAuthenticationStateAsync());
    }
}