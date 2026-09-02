using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ExpenseTracker.UI.Services;

public class ApiHttpClient
{
    private readonly HttpClient _http;
    private readonly AuthStateService _authState;
    private readonly NavigationManager _navigation;

    public ApiHttpClient(
        AuthStateService authState,
        NavigationManager navigation)
    {
        _authState = authState;
        _navigation = navigation;

        _http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7135/")
        };
    }

    private async Task PrepareClientAsync()
    {
        var token = await _authState.GetTokenAsync();

        _http.DefaultRequestHeaders.Remove("Authorization");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request)
    {
        await PrepareClientAsync();

        var response =
            await _http.SendAsync(request);

        if (response.StatusCode ==
            System.Net.HttpStatusCode.Unauthorized)
        {
            await _authState.LogoutAsync();

            _navigation.NavigateTo(
                "/login",
                forceLoad: true);
        }

        return response;
    }

    public async Task<HttpResponseMessage> GetAsync(
        string requestUri)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                requestUri);

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> PostAsync(
        string requestUri,
        HttpContent? content = null)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                requestUri)
            {
                Content = content
            };

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        string requestUri,
        T value)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                requestUri)
            {
                Content =
                    JsonContent.Create(value)
            };

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> PutAsync(
        string requestUri,
        HttpContent? content = null)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Put,
                requestUri)
            {
                Content = content
            };

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(
        string requestUri,
        T value)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Put,
                requestUri)
            {
                Content =
                    JsonContent.Create(value)
            };

        return await SendAsync(request);
    }

    public async Task<HttpResponseMessage> DeleteAsync(
        string requestUri)
    {
        var request =
            new HttpRequestMessage(
                HttpMethod.Delete,
                requestUri);

        return await SendAsync(request);
    }
}