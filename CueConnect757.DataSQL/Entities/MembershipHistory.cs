namespace CueConnect757.DataSQL.Entities;

public class MembershipHistory
{
    public int Id { get; set; }

    public int PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public int Year { get; set; }
    public string LeagueName { get; set; } = string.Empty;
}