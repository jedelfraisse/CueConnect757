using CueConnect757.Core.Models;
using CueConnect757.DataSQL.Entities;

namespace CueConnect757.Core.Mappers;

public class DivisionMapper
{
    public static Division MapToEntity(ApaDivisionModel model, int sessionId)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new Division
        {
            DivisionIdFromAPA = model.DivisionId,
            Name = model.Name?.Trim() ?? string.Empty,
            NightOfPlay = model.NightOfPlay?.Trim() ?? string.Empty,
            Format = model.Format?.Trim() ?? string.Empty,
            SessionId = sessionId
        };
    }

    public static void ApplyToEntity(ApaDivisionModel model, Division entity, int sessionId)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);

        entity.DivisionIdFromAPA = model.DivisionId;
        entity.Name = model.Name?.Trim() ?? string.Empty;
        entity.NightOfPlay = model.NightOfPlay?.Trim() ?? string.Empty;
        entity.Format = model.Format?.Trim() ?? string.Empty;
        entity.SessionId = sessionId;
    }
}
