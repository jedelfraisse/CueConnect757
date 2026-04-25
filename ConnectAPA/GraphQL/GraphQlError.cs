using System.Text.Json.Serialization;

namespace ConnectAPA.GraphQL;

public sealed class GraphQlError
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}
