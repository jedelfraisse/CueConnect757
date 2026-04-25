using System.Reflection;
using System.Text.Json.Serialization;
using ConnectAPA.Models.Public;

namespace ConnectAPA.Public;

public static class PublicApaExtensions
{
    private static readonly string QueriesPath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
        "Public", "Queries");

    public static async Task<League> GetLeagueLayoutAsync(this PublicApaClient client, string slug)
    {
        var query = await LoadQueryAsync("LeagueLayout.graphql");
        var response = await client.SendAsync<LeagueResponse>("LeagueLayout", query, new { slug });
        
        return response.League ?? throw new InvalidOperationException($"League '{slug}' not found.");
    }

    public static async Task<League> GetLeagueInfoAsync(this PublicApaClient client, string slug)
    {
        var query = await LoadQueryAsync("LeagueInfo.graphql");
        var response = await client.SendAsync<LeagueResponse>("LeagueInfo", query, new { slug });
        
        return response.League ?? throw new InvalidOperationException($"League '{slug}' not found.");
    }

    public static async Task<List<Division>> GetDivisionsAsync(this PublicApaClient client, string slug)
    {
        var query = await LoadQueryAsync("Divisions.graphql");
        var response = await client.SendAsync<LeagueResponse>("Divisions", query, new { slug });
        
        if (response.League is null)
        {
            throw new InvalidOperationException($"League '{slug}' not found.");
        }

        return response.League.Divisions;
    }

    public static async Task<List<Match>> GetScheduleAsync(this PublicApaClient client, int divisionId)
    {
        var query = await LoadQueryAsync("Schedule.graphql");
        var response = await client.SendAsync<DivisionResponse>("Schedule", query, new { divisionId });
        
        if (response.Division is null)
        {
            throw new InvalidOperationException($"Division '{divisionId}' not found.");
        }

        return response.Division.Matches;
    }

    public static async Task<List<Team>> GetTeamsAsync(this PublicApaClient client, int divisionId)
    {
        var query = await LoadQueryAsync("Teams.graphql");
        var response = await client.SendAsync<DivisionResponse>("Teams", query, new { divisionId });
        
        if (response.Division is null)
        {
            throw new InvalidOperationException($"Division '{divisionId}' not found.");
        }

        return response.Division.Teams;
    }

    public static async Task<List<LeaguePost>> GetLeaguePostsAsync(this PublicApaClient client, string slug, int limit, int offset)
    {
        var query = await LoadQueryAsync("LeaguePosts.graphql");
        var response = await client.SendAsync<LeagueResponse>("LeaguePosts", query, new { slug, limit, offset });
        
        if (response.League is null)
        {
            throw new InvalidOperationException($"League '{slug}' not found.");
        }

        return response.League.News;
    }

    private static async Task<string> LoadQueryAsync(string fileName)
    {
        var filePath = Path.Combine(QueriesPath, fileName);
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"GraphQL query file not found: {filePath}");
        }

        return await File.ReadAllTextAsync(filePath);
    }

    private sealed class LeagueResponse
    {
        [JsonPropertyName("league")]
        public League? League { get; set; }
    }

    private sealed class DivisionResponse
    {
        [JsonPropertyName("division")]
        public Division? Division { get; set; }
    }
}
