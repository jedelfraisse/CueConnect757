# Quick Start Guide - User Profiles & Linked Accounts

## Apply Database Migration

```bash
cd CueConnect757.DataSQL
dotnet ef database update --startup-project ../CueConnect757
```

## Test the Features

### 1. Test User Menu
1. Run the app: `dotnet run`
2. Sign in with Microsoft account
3. Click your avatar in top-right
4. Dropdown should show:
   - Your name
   - Your email
   - Profile link
   - Linked Accounts link
   - Sign out button

### 2. Test Profile Page
1. Click "Profile" from dropdown
2. View your Microsoft identity information (read-only)
3. Edit local settings:
   - Enter a nickname
   - Set preferred display name
   - Choose time zone
   - Toggle notification preferences
4. Click "Save Changes"
5. Refresh page - changes should persist

### 3. Test Linked Accounts
1. Click avatar → "Linked Accounts"
2. See Microsoft account (always connected)
3. See APA account card
4. Click "Connect APA Account"
   - Redirects to APA login
   - Sign in with APA credentials
   - Redirects back with token
   - Should show "Connected" status
5. Click "Unlink Account" to disconnect

## Key Files Created

### Services
- `Services/UserProfileService.cs` - Profile management
- `Services/ApaLinkingService.cs` - APA OAuth integration

### Pages
- `Components/Pages/Profile.razor` - User profile editor
- `Components/Pages/LinkedAccounts.razor` - Account connections

### Database
- `CueConnect757.DataSQL/Entities/UserProfile.cs` - Data model
- Migration: `AddUserProfiles` - Database schema

### Updated
- `Components/Shared/LoginDisplay.razor` - Fixed dropdown
- `Program.cs` - Service registration
- `CueConnect757.DataSQL/CueConnectDbContext.cs` - DbSet added

## APA Integration

### Getting APA Access Token
```csharp
@inject ApaLinkingService ApaLinking

var accessToken = await ApaLinking.GetAccessTokenForUserAsync(userId);
```

### Executing APA GraphQL Query
```csharp
var stats = await ApaLinking.ExecuteApaGraphQlQueryAsync<ApaStats>(
    userId,
    "query { viewer { id name eightBallStats { skillLevel } } }"
);
```

### Checking if APA is Linked
```csharp
@inject UserProfileService ProfileService

if (await ProfileService.IsApaLinkedAsync(userId))
{
    // User has connected their APA account
}
```

## Troubleshooting

**Dropdown not opening?**
- Clear browser cache
- Check browser console for errors

**Database errors?**
- Run: `dotnet ef database update --startup-project ../CueConnect757`
- Verify SQL Server is running

**APA OAuth not working?**
- Must use HTTPS
- Verify redirect URI in APA app settings
- Check that redirect URI matches exactly

## Security Notes

### In Production:
1. **Encrypt APA tokens** in database
2. **Use Azure Key Vault** for connection strings
3. **Enable SSL/TLS** for all connections
4. **Add rate limiting** to OAuth endpoints
5. **Implement token rotation**

### Best Practices:
- Never log refresh tokens
- Always use HTTPS for OAuth
- Validate all user input
- Use parameterized queries (EF handles this)

## What's Next?

### Immediate:
1. Apply database migration
2. Test all features
3. Verify APA OAuth flow

### Future Enhancements:
- Facebook login
- Google login
- Profile pictures
- Privacy settings
- Two-factor authentication
- Password-less login options

## Quick Commands

```bash
# Build project
dotnet build

# Run app
dotnet run

# Apply migration
cd CueConnect757.DataSQL
dotnet ef database update --startup-project ../CueConnect757

# Create new migration
dotnet ef migrations add MyMigrationName --startup-project ../CueConnect757

# Rollback last migration
dotnet ef migrations remove --startup-project ../CueConnect757

# View database changes
dotnet ef migrations script --startup-project ../CueConnect757
```

## Support & Documentation

- Full documentation: `USER_PROFILE_IMPLEMENTATION.md`
- Authentication setup: `AUTHENTICATION_SETUP.md`
- Quick Azure AD setup: `QUICK_SETUP.md`

## Status

✅ All features implemented
✅ Build successful
✅ Services registered
✅ Migration created
✅ Ready to test

Run `dotnet ef database update` and start testing!
