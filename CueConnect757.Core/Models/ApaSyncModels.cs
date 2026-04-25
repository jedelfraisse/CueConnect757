namespace CueConnect757.Core.Models;

public sealed class ApaSessionModel
{
    public int SessionId { get; set; }
    public string? Name { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public sealed class ApaDivisionModel
{
    public int DivisionId { get; set; }
    public string? Name { get; set; }
    public string? NightOfPlay { get; set; }
    public string? Format { get; set; }
}

public sealed class ApaTeamModel
{
    public int TeamId { get; set; }
    public string? Name { get; set; }
    public string? Number { get; set; }
}

public sealed class ApaMatchModel
{
    public int MatchId { get; set; }
    public DateTime? MatchDate { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }
}

public sealed class ApaPlayerModel
{
    public int AliasId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int SkillLevel { get; set; }

    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int BreakAndRuns { get; set; }
    public int Rackless { get; set; }

    public List<ApaMembershipModel> MembershipHistory { get; set; } = [];
}

public sealed class ApaMembershipModel
{
    public int Year { get; set; }
    public string? LeagueName { get; set; }
}
