using System.Text.Json.Serialization;

namespace ConnectAPA.GraphQL;

public sealed class GraphQlRequest
{
    [JsonPropertyName("operationName")]
    public string OperationName { get; set; } = string.Empty;

    [JsonPropertyName("query")]
    public string Query { get; set; } = string.Empty;

    [JsonPropertyName("variables")]
    public object? Variables { get; set; }
}
