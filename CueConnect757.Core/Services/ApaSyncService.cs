using ConnectAPA.GraphQL;
using CueConnect757.Core.Mappers;
using CueConnect757.Core.Models;
using CueConnect757.DataSQL;
using CueConnect757.DataSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CueConnect757.Core.Services;

public class ApaSyncService : IApaSyncService
{
    private readonly ApaGraphQlClient _apaClient;
    private readonly CueConnectDbContext _dbContext;

    public ApaSyncService(ApaGraphQlClient apaClient, CueConnectDbContext dbContext)
    {
        _apaClient = apaClient;
        _dbContext = dbContext;
    }

    public async Task SyncHistoricalSessionsAsync()
    {
        var sessions = await FetchSessionsAsync();
        if (sessions.Count == 0)
        {
            return;
        }

        foreach (var session in sessions.OrderBy(s => s.StartDate ?? DateTime.MinValue))
        {
            await SyncSessionInternalAsync(session);
        }
    }

    public async Task SyncCurrentSessionAsync()
    {
        var sessions = await FetchSessionsAsync();
        var currentSession = sessions
            .OrderByDescending(s => s.StartDate ?? DateTime.MinValue)
            .FirstOrDefault();

        if (currentSession is null)
        {
            return;
        }

        await SyncSessionInternalAsync(currentSession);
    }

    public async Task SyncSessionAsync(int sessionId)
    {
        var session = await FetchSessionAsync(sessionId);
        if (session is null)
        {
            return;
        }

        await SyncSessionInternalAsync(session);
    }

    private async Task SyncSessionInternalAsync(ApaSessionModel sessionModel)
    {
        var sessionEntity = await UpsertSessionAsync(sessionModel);

        var divisions = await FetchDivisionsAsync(sessionModel.SessionId);
        foreach (var divisionModel in divisions)
        {
            var divisionEntity = await UpsertDivisionAsync(sessionEntity.Id, divisionModel);

            var teams = await FetchTeamsAsync(divisionModel.DivisionId);
            var teamLookup = await UpsertTeamsAsync(divisionEntity.Id, teams);

            var matches = await FetchMatchesAsync(divisionModel.DivisionId);
            await UpsertMatchesAsync(divisionEntity.Id, teamLookup, matches);
        }

        var players = await FetchPlayersAsync(sessionModel.SessionId);
        await UpsertPlayersAndStatsAsync(sessionEntity.Id, players);

        await _dbContext.SaveChangesAsync();
    }

    private async Task<Session> UpsertSessionAsync(ApaSessionModel model)
    {
        var existing = await _dbContext.Sessions
            .FirstOrDefaultAsync(x => x.SessionIdFromAPA == model.SessionId);

        if (existing is null)
        {
            var entity = SessionMapper.MapToEntity(model);
            _dbContext.Sessions.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        SessionMapper.ApplyToEntity(model, existing);
        await _dbContext.SaveChangesAsync();
        return existing;
    }

    private async Task<Division> UpsertDivisionAsync(int sessionId, ApaDivisionModel model)
    {
        var existing = await _dbContext.Divisions
            .FirstOrDefaultAsync(x => x.DivisionIdFromAPA == model.DivisionId && x.SessionId == sessionId);

        if (existing is null)
        {
            var entity = DivisionMapper.MapToEntity(model, sessionId);
            _dbContext.Divisions.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        DivisionMapper.ApplyToEntity(model, existing, sessionId);
        await _dbContext.SaveChangesAsync();
        return existing;
    }

    private async Task<Dictionary<int, Team>> UpsertTeamsAsync(int divisionId, IReadOnlyCollection<ApaTeamModel> models)
    {
        var apaIds = models.Select(x => x.TeamId).ToHashSet();

        var existingTeams = await _dbContext.Teams
            .Where(t => t.DivisionId == divisionId && apaIds.Contains(t.TeamIdFromAPA))
            .ToDictionaryAsync(t => t.TeamIdFromAPA);

        foreach (var model in models)
        {
            if (!existingTeams.TryGetValue(model.TeamId, out var teamEntity))
            {
                teamEntity = TeamMapper.MapToEntity(model, divisionId);
                _dbContext.Teams.Add(teamEntity);
                existingTeams[model.TeamId] = teamEntity;
                continue;
            }

            TeamMapper.ApplyToEntity(model, teamEntity, divisionId);
        }

        await _dbContext.SaveChangesAsync();

        return await _dbContext.Teams
            .Where(t => t.DivisionId == divisionId && apaIds.Contains(t.TeamIdFromAPA))
            .ToDictionaryAsync(t => t.TeamIdFromAPA);
    }

    private async Task UpsertMatchesAsync(int divisionId, IReadOnlyDictionary<int, Team> teamLookup, IReadOnlyCollection<ApaMatchModel> models)
    {
        var apaIds = models.Select(x => x.MatchId).ToHashSet();
        var existingMatches = await _dbContext.Matches
            .Where(m => m.DivisionId == divisionId && apaIds.Contains(m.MatchIdFromAPA))
            .ToDictionaryAsync(m => m.MatchIdFromAPA);

        foreach (var model in models)
        {
            if (!teamLookup.TryGetValue(model.HomeTeamId, out var homeTeam) || !teamLookup.TryGetValue(model.AwayTeamId, out var awayTeam))
            {
                continue;
            }

            if (!existingMatches.TryGetValue(model.MatchId, out var matchEntity))
            {
                matchEntity = MatchMapper.MapToEntity(model, divisionId, homeTeam.Id, awayTeam.Id);
                _dbContext.Matches.Add(matchEntity);
                continue;
            }

            MatchMapper.ApplyToEntity(model, matchEntity, divisionId, homeTeam.Id, awayTeam.Id);
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task UpsertPlayersAndStatsAsync(int sessionId, IReadOnlyCollection<ApaPlayerModel> models)
    {
        var playerAliasIds = models.Select(x => x.AliasId).ToHashSet();

        var existingPlayers = await _dbContext.Players
            .Where(p => playerAliasIds.Contains(p.AliasId))
            .ToDictionaryAsync(p => p.AliasId);

        foreach (var model in models)
        {
            if (!existingPlayers.TryGetValue(model.AliasId, out var playerEntity))
            {
                playerEntity = PlayerMapper.MapToEntity(model);
                _dbContext.Players.Add(playerEntity);
                existingPlayers[model.AliasId] = playerEntity;
                continue;
            }

            PlayerMapper.ApplyToEntity(model, playerEntity);
        }

        await _dbContext.SaveChangesAsync();

        var persistedPlayers = await _dbContext.Players
            .Where(p => playerAliasIds.Contains(p.AliasId))
            .ToDictionaryAsync(p => p.AliasId);

        foreach (var model in models)
        {
            if (!persistedPlayers.TryGetValue(model.AliasId, out var playerEntity))
            {
                continue;
            }

            var statsEntity = await _dbContext.PlayerSessionStats
                .FirstOrDefaultAsync(s => s.PlayerId == playerEntity.Id && s.SessionId == sessionId);

            if (statsEntity is null)
            {
                _dbContext.PlayerSessionStats.Add(PlayerMapper.MapStatsToEntity(model, playerEntity.Id, sessionId));
            }
            else
            {
                statsEntity.MatchesPlayed = model.MatchesPlayed;
                statsEntity.MatchesWon = model.MatchesWon;
                statsEntity.BreakAndRuns = model.BreakAndRuns;
                statsEntity.Rackless = model.Rackless;
            }

            foreach (var membershipModel in model.MembershipHistory)
            {
                var membershipEntity = await _dbContext.MembershipHistories
                    .FirstOrDefaultAsync(m => m.PlayerId == playerEntity.Id
                        && m.Year == membershipModel.Year
                        && m.LeagueName == (membershipModel.LeagueName ?? string.Empty));

                if (membershipEntity is not null)
                {
                    continue;
                }

                _dbContext.MembershipHistories.Add(PlayerMapper.MapMembershipToEntity(membershipModel, playerEntity.Id));
            }
        }

        await _dbContext.SaveChangesAsync();
    }

    private async Task<IReadOnlyCollection<ApaSessionModel>> FetchSessionsAsync()
    {
        var query = """
            query Sessions {
              sessions {
                sessionId
                name
                startDate
                endDate
              }
            }
            """;

        var response = await _apaClient.SendAsync<SessionsResponse>(query, new { });
        return response.Sessions ?? [];
    }

    private async Task<ApaSessionModel?> FetchSessionAsync(int sessionId)
    {
        var query = """
            query SessionById($sessionId: Int!) {
              session(sessionId: $sessionId) {
                sessionId
                name
                startDate
                endDate
              }
            }
            """;

        var response = await _apaClient.SendAsync<SessionResponse>(query, new { sessionId });
        return response.Session;
    }

    private async Task<IReadOnlyCollection<ApaDivisionModel>> FetchDivisionsAsync(int sessionId)
    {
        var query = """
            query Divisions($sessionId: Int!) {
              divisions(sessionId: $sessionId) {
                divisionId
                name
                nightOfPlay
                format
              }
            }
            """;

        var response = await _apaClient.SendAsync<DivisionsResponse>(query, new { sessionId });
        return response.Divisions ?? [];
    }

    private async Task<IReadOnlyCollection<ApaTeamModel>> FetchTeamsAsync(int divisionId)
    {
        var query = """
            query Teams($divisionId: Int!) {
              teams(divisionId: $divisionId) {
                teamId
                name
                number
              }
            }
            """;

        var response = await _apaClient.SendAsync<TeamsResponse>(query, new { divisionId });
        return response.Teams ?? [];
    }

    private async Task<IReadOnlyCollection<ApaMatchModel>> FetchMatchesAsync(int divisionId)
    {
        var query = """
            query Matches($divisionId: Int!) {
              matches(divisionId: $divisionId) {
                matchId
                matchDate
                homeTeamId
                awayTeamId
              }
            }
            """;

        var response = await _apaClient.SendAsync<MatchesResponse>(query, new { divisionId });
        return response.Matches ?? [];
    }

    private async Task<IReadOnlyCollection<ApaPlayerModel>> FetchPlayersAsync(int sessionId)
    {
        var query = """
            query SessionPlayers($sessionId: Int!) {
              players(sessionId: $sessionId) {
                aliasId
                firstName
                lastName
                skillLevel
                matchesPlayed
                matchesWon
                breakAndRuns
                rackless
                membershipHistory {
                  year
                  leagueName
                }
              }
            }
            """;

        var response = await _apaClient.SendAsync<PlayersResponse>(query, new { sessionId });
        return response.Players ?? [];
    }

    private sealed class SessionsResponse
    {
        public List<ApaSessionModel>? Sessions { get; set; }
    }

    private sealed class SessionResponse
    {
        public ApaSessionModel? Session { get; set; }
    }

    private sealed class DivisionsResponse
    {
        public List<ApaDivisionModel>? Divisions { get; set; }
    }

    private sealed class TeamsResponse
    {
        public List<ApaTeamModel>? Teams { get; set; }
    }

    private sealed class MatchesResponse
    {
        public List<ApaMatchModel>? Matches { get; set; }
    }

    private sealed class PlayersResponse
    {
        public List<ApaPlayerModel>? Players { get; set; }
    }
}
