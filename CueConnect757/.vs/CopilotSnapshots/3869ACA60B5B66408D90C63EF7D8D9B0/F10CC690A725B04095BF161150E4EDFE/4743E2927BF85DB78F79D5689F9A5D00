using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace ConnectAPA.GraphQL;

public class ApaGraphQlClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _http;

    public ApaGraphQlClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<T> SendAsync<T>(string query, object variables)
    {
        var request = new GraphQlRequest
        {
            OperationName = ExtractOperationName(query),
            Query = query,
            Variables = variables
        };

        var responses = await SendBatchAsync<T>([request]);
        if (responses.Length == 0 || responses[0] is null)
        {
            throw new InvalidOperationException("APA returned an empty response.");
        }

        return responses[0];
    }

    public async Task<T[]> SendBatchAsync<T>(IEnumerable<GraphQlRequest> requests)
    {
        EnsureAuthorizationConfigured();

        using var response = await _http.PostAsJsonAsync(string.Empty, requests, JsonOptions);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"APA request failed with status {(int)response.StatusCode}: {responseBody}");
        }

        var graphQlResponses = JsonSerializer.Deserialize<GraphQlResponse<T>[]>(responseBody, JsonOptions)
            ?? throw new InvalidOperationException("APA returned a null response payload.");

        var errors = graphQlResponses
            .SelectMany(x => x.Errors)
            .Where(x => !string.IsNullOrWhiteSpace(x.Message))
            .Select(x => x.Message)
            .ToArray();

        if (errors.Length > 0)
        {
            throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
        }

        return graphQlResponses.Select(x => x.Data).Where(x => x is not null).Cast<T>().ToArray();
    }

    private void EnsureAuthorizationConfigured()
    {
        if (_http.DefaultRequestHeaders.Authorization is AuthenticationHeaderValue { Scheme: "Bearer", Parameter.Length: > 0 })
        {
            return;
        }

        throw new InvalidOperationException("APA access token is not configured. Set `Apa:AccessToken` in configuration.");
    }

    private static string ExtractOperationName(string query)
    {
        var parts = query
            .Split([' ', '\r', '\n', '\t', '(', '{'], StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2 && string.Equals(parts[0], "query", StringComparison.OrdinalIgnoreCase))
        {
            return parts[1];
        }

        return "AnonymousQuery";
    }
}
