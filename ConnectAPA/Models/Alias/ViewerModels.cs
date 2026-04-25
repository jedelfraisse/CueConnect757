namespace ConnectAPA.Models.Alias;

public sealed class ViewerResponse
{
    public Viewer? Viewer { get; set; }
}

public sealed class Viewer
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<PlayerAlias> Aliases { get; set; } = [];
}

public sealed class PlayerAlias
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int SkillLevel { get; set; }
    public string PlaysSince { get; set; } = string.Empty;
    public List<AliasTeam> Teams { get; set; } = [];
}

public sealed class AliasTeam
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
