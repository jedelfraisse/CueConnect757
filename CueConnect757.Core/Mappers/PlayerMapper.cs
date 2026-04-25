using CueConnect757.Core.Models;
using CueConnect757.DataSQL.Entities;

namespace CueConnect757.Core.Mappers;

public class PlayerMapper
{
    public static Player MapToEntity(ApaPlayerModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new Player
        {
            AliasId = model.AliasId,
            FirstName = model.FirstName?.Trim() ?? string.Empty,
            LastName = model.LastName?.Trim() ?? string.Empty,
            SkillLevel = model.SkillLevel
        };
    }

    public static void ApplyToEntity(ApaPlayerModel model, Player entity)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);

        entity.AliasId = model.AliasId;
        entity.FirstName = model.FirstName?.Trim() ?? string.Empty;
        entity.LastName = model.LastName?.Trim() ?? string.Empty;
        entity.SkillLevel = model.SkillLevel;
    }

    public static PlayerSessionStats MapStatsToEntity(ApaPlayerModel model, int playerId, int sessionId)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new PlayerSessionStats
        {
            PlayerId = playerId,
            SessionId = sessionId,
            MatchesPlayed = model.MatchesPlayed,
            MatchesWon = model.MatchesWon,
            BreakAndRuns = model.BreakAndRuns,
            Rackless = model.Rackless
        };
    }

    public static MembershipHistory MapMembershipToEntity(ApaMembershipModel model, int playerId)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new MembershipHistory
        {
            PlayerId = playerId,
            Year = model.Year,
            LeagueName = model.LeagueName?.Trim() ?? string.Empty
        };
    }
}
