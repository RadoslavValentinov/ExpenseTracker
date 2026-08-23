namespace ExpenseTracker.UI.Services;

public class ApiHttpClient
{
    private readonly HttpClient _http;
    private readonly AuthStateService _authState;

    public ApiHttpClient(
        AuthStateService authState)
    {
        _authState = authState;

        _http = new HttpClient
        {
            BaseAddress = new Uri("https://localhost:7135/")
        };
    }


    private async Task<HttpClient> GetClientAsync()
    {
        var token =
            await _authState.GetTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            _http.DefaultRequestHeaders.Remove("Authorization");

            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer",
                    token);
        }
        else
        {
            _http.DefaultRequestHeaders.Remove("Authorization");
        }

        return _http;
    }


    public async Task<HttpResponseMessage> GetAsync(
        string requestUri)
    {
        var client = await GetClientAsync();

        return await client.GetAsync(requestUri);
    }


    public async Task<HttpResponseMessage> PostAsync(
        string requestUri,
        HttpContent? content = null)
    {
        var client = await GetClientAsync();

        return await client.PostAsync(
            requestUri,
            content);
    }


    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(
        string requestUri,
        T value)
    {
        var client = await GetClientAsync();

        return await client.PostAsJsonAsync(
            requestUri,
            value);
    }


    public async Task<HttpResponseMessage> PutAsync(
        string requestUri,
        HttpContent? content = null)
    {
        var client = await GetClientAsync();

        return await client.PutAsync(
            requestUri,
            content);
    }


    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(
        string requestUri,
        T value)
    {
        var client = await GetClientAsync();

        return await client.PutAsJsonAsync(
            requestUri,
            value);
    }


    public async Task<HttpResponseMessage> DeleteAsync(
        string requestUri)
    {
        var client = await GetClientAsync();

        return await client.DeleteAsync(requestUri);
    }
}