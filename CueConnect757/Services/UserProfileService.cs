using CueConnect757.DataSQL;
using CueConnect757.DataSQL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CueConnect757.Services;

public class UserProfileService
{
    private readonly CueConnectDbContext _dbContext;

    public UserProfileService(CueConnectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfile?> GetUserProfileAsync(string userId)
    {
        return await _dbContext.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);
    }

    public async Task<UserProfile> GetOrCreateUserProfileAsync(string userId)
    {
        var profile = await GetUserProfileAsync(userId);
        
        if (profile == null)
        {
            profile = new UserProfile
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            _dbContext.UserProfiles.Add(profile);
            await _dbContext.SaveChangesAsync();
        }
        
        return profile;
    }

    public async Task<UserProfile> UpdateUserProfileAsync(UserProfile profile)
    {
        profile.UpdatedAt = DateTime.UtcNow;
        _dbContext.UserProfiles.Update(profile);
        await _dbContext.SaveChangesAsync();
        return profile;
    }

    public async Task<bool> LinkApaAccountAsync(string userId, string refreshToken)
    {
        var profile = await GetOrCreateUserProfileAsync(userId);
        profile.ApaRefreshToken = refreshToken;
        profile.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlinkApaAccountAsync(string userId)
    {
        var profile = await GetUserProfileAsync(userId);
        
        if (profile == null)
            return false;
        
        profile.ApaRefreshToken = null;
        profile.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsApaLinkedAsync(string userId)
    {
        var profile = await GetUserProfileAsync(userId);
        return !string.IsNullOrWhiteSpace(profile?.ApaRefreshToken);
    }

    public async Task<string?> GetApaRefreshTokenAsync(string userId)
    {
        var profile = await GetUserProfileAsync(userId);
        return profile?.ApaRefreshToken;
    }

    public async Task<T?> GetNotificationPreferencesAsync<T>(string userId) where T : class
    {
        var profile = await GetUserProfileAsync(userId);
        
        if (profile?.NotificationPreferences == null)
            return null;
        
        try
        {
            return JsonSerializer.Deserialize<T>(profile.NotificationPreferences);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetNotificationPreferencesAsync<T>(string userId, T preferences) where T : class
    {
        var profile = await GetOrCreateUserProfileAsync(userId);
        profile.NotificationPreferences = JsonSerializer.Serialize(preferences);
        profile.UpdatedAt = DateTime.UtcNow;
        
        await _dbContext.SaveChangesAsync();
    }
}
