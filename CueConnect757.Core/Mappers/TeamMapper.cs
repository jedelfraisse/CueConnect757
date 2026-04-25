using CueConnect757.Core.Models;
using CueConnect757.DataSQL.Entities;

namespace CueConnect757.Core.Mappers;

public class TeamMapper
{
    public static Team MapToEntity(ApaTeamModel model, int divisionId)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new Team
        {
            TeamIdFromAPA = model.TeamId,
            Name = model.Name?.Trim() ?? string.Empty,
            Number = model.Number?.Trim() ?? string.Empty,
            DivisionId = divisionId
        };
    }

    public static void ApplyToEntity(ApaTeamModel model, Team entity, int divisionId)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);

        entity.TeamIdFromAPA = model.TeamId;
        entity.Name = model.Name?.Trim() ?? string.Empty;
        entity.Number = model.Number?.Trim() ?? string.Empty;
        entity.DivisionId = divisionId;
    }
}
