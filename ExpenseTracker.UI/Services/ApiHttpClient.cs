using Microsoft.AspNetCore.Components;
using System.Net;
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


    // =========================
    // SEND
    // =========================

    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request)
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


        HttpResponseMessage response;

        try
        {
            response =
                await _http.SendAsync(request);
        }
        catch (HttpRequestException)
        {
            throw;
        }


        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await _authState.LogoutAsync();


            _navigation.NavigateTo(
                "/login",
                forceLoad: true);
        }


        return response;
    }


    // =========================
    // GET
    // =========================

    public async Task<HttpResponseMessage> GetAsync(
        string requestUri)
    {
        using var request =
            new HttpRequestMessage(
                HttpMethod.Get,
                requestUri);


        return await SendAsync(request);
    }


    // =========================
    // POST
    // =========================

    public async Task<HttpResponseMessage> PostAsync(
        string requestUri,
        HttpContent? content = null)
    {
        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                requestUri)
            {
                Content = content
            };


        return await SendAsync(request);
    }


    // =========================
    // POST JSON
    // =========================

    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        string requestUri,
        T value)
    {
        using var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                requestUri)
            {
                Content =
                    JsonContent.Create(value)
            };


        return await SendAsync(request);
    }


    // =========================
    // PUT
    // =========================

    public async Task<HttpResponseMessage> PutAsync(
        string requestUri,
        HttpContent? content = null)
    {
        using var request =
            new HttpRequestMessage(
                HttpMethod.Put,
                requestUri)
            {
                Content = content
            };


        return await SendAsync(request);
    }


    // =========================
    // PUT JSON
    // =========================

    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(
        string requestUri,
        T value)
    {
        using var request =
            new HttpRequestMessage(
                HttpMethod.Put,
                requestUri)
            {
                Content =
                    JsonContent.Create(value)
            };


        return await SendAsync(request);
    }


    // =========================
    // DELETE
    // =========================

    public async Task<HttpResponseMessage> DeleteAsync(
        string requestUri)
    {
        using var request =
            new HttpRequestMessage(
                HttpMethod.Delete,
                requestUri);


        return await SendAsync(request);
    }
}