namespace CueConnect757.Core.Services;

public interface IApaSyncService
{
    Task SyncHistoricalSessionsAsync();
    Task SyncCurrentSessionAsync();
    Task SyncSessionAsync(int sessionId);
}
