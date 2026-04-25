using CueConnect757.Core.Models;
using CueConnect757.DataSQL.Entities;

namespace CueConnect757.Core.Mappers;

public class MatchMapper
{
    public static Match MapToEntity(ApaMatchModel model, int divisionId, int homeTeamId, int awayTeamId)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new Match
        {
            MatchIdFromAPA = model.MatchId,
            MatchDate = model.MatchDate.HasValue ? DateOnly.FromDateTime(model.MatchDate.Value) : DateOnly.MinValue,
            DivisionId = divisionId,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId
        };
    }

    public static void ApplyToEntity(ApaMatchModel model, Match entity, int divisionId, int homeTeamId, int awayTeamId)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);

        entity.MatchIdFromAPA = model.MatchId;
        entity.MatchDate = model.MatchDate.HasValue ? DateOnly.FromDateTime(model.MatchDate.Value) : DateOnly.MinValue;
        entity.DivisionId = divisionId;
        entity.HomeTeamId = homeTeamId;
        entity.AwayTeamId = awayTeamId;
    }
}
