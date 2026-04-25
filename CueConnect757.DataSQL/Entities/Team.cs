namespace CueConnect757.DataSQL.Entities;

public class Team
{
    public int Id { get; set; }
    public int TeamIdFromAPA { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;

    public int DivisionId { get; set; }
    public Division Division { get; set; } = null!;

    public DateTime? LastSyncedAt { get; set; }
    public string SyncSource { get; set; } = "APA";

    public ICollection<Match> HomeMatches { get; set; } = new List<Match>();
    public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
}