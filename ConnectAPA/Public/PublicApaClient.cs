using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ConnectAPA.GraphQL;

namespace ConnectAPA.Public;

public class PublicApaClient
{
	private readonly HttpClient _http;
	private static readonly JsonSerializerOptions _jsonOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	};

	public PublicApaClient(HttpClient http)
	{
		_http = http;
	}

	public async Task<T> SendAsync<T>(string operationName, string query, object? variables)
	{
		var request = new GraphQlRequest
		{
			OperationName = operationName,
			Query = query,
			Variables = variables
		};

		var batch = new[] { request };
		return (await SendBatchAsync<T>(batch)).First();
	}

	public async Task<T[]> SendBatchAsync<T>(IEnumerable<GraphQlRequest> requests)
	{
		var payload = JsonSerializer.Serialize(requests, _jsonOptions);
		var content = new StringContent(payload, Encoding.UTF8, "application/json");

		var response = await _http.PostAsync("https://gql.poolplayers.com/graphql", content);
		if (!response.IsSuccessStatusCode)
			throw new Exception($"HTTP {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

		var json = await response.Content.ReadAsStringAsync();
		var result = JsonSerializer.Deserialize<GraphQlResponse<T>[]>(json, _jsonOptions);

		if (result == null)
			throw new Exception("Failed to deserialize GraphQL response.");

		foreach (var r in result)
		{
			if (r.Errors?.Length > 0)
				throw new Exception(string.Join("; ", r.Errors.Select(e => e.Message)));
		}

		return result.Select(r => r.Data!).ToArray();
	}
}