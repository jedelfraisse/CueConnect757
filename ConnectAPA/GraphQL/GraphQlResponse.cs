using System.Text.Json.Serialization;

namespace ConnectAPA.GraphQL;

public sealed class GraphQlResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    public GraphQlError[] Errors { get; set; } = [];
}
