namespace CueConnect757.DataSQL.Entities;

public class PlayerSessionStats
{
    public int Id { get; set; }

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int SessionId { get; set; }
    public Session Session { get; set; } = null!;

    public int MatchesPlayed { get; set; }
    public int MatchesWon { get; set; }
    public int BreakAndRuns { get; set; }
    public int Rackless { get; set; }
}