using CueConnect757.Core.Models;
using CueConnect757.DataSQL.Entities;

namespace CueConnect757.Core.Mappers;

public class SessionMapper
{
    public static Session MapToEntity(ApaSessionModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new Session
        {
            SessionIdFromAPA = model.SessionId,
            Name = model.Name?.Trim() ?? string.Empty,
            StartDate = model.StartDate.HasValue ? DateOnly.FromDateTime(model.StartDate.Value) : DateOnly.MinValue,
            EndDate = model.EndDate.HasValue ? DateOnly.FromDateTime(model.EndDate.Value) : DateOnly.MinValue
        };
    }

    public static void ApplyToEntity(ApaSessionModel model, Session entity)
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);

        entity.SessionIdFromAPA = model.SessionId;
        entity.Name = model.Name?.Trim() ?? string.Empty;
        entity.StartDate = model.StartDate.HasValue ? DateOnly.FromDateTime(model.StartDate.Value) : DateOnly.MinValue;
        entity.EndDate = model.EndDate.HasValue ? DateOnly.FromDateTime(model.EndDate.Value) : DateOnly.MinValue;
    }
}
