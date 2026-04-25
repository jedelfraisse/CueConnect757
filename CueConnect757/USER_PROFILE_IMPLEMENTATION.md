# User Profile and Linked Accounts Implementation

## Overview
This implementation adds comprehensive user profile management and external account linking functionality to CueConnect757, including APA account integration.

## What Was Implemented

### 1. Database Model
**File:** `CueConnect757.DataSQL/Entities/UserProfile.cs`

New table: `UserProfiles`
- `Id` (GUID) - Primary key
- `UserId` (string) - Microsoft Entra ID user identifier
- `Nickname` (string, nullable) - User's preferred nickname
- `PreferredDisplayName` (string, nullable) - Display name override
- `TimeZone` (string, nullable) - User's time zone preference
- `NotificationPreferences` (JSON string, nullable) - Notification settings
- `ApaRefreshToken` (string, nullable) - Encrypted APA OAuth refresh token
- `CreatedAt` (DateTime) - Record creation timestamp
- `UpdatedAt` (DateTime) - Last modification timestamp

**Migration:** `AddUserProfiles`
```bash
# To apply the migration:
cd CueConnect757.DataSQL
dotnet ef database update --startup-project ../CueConnect757
```

### 2. Service Classes

#### UserProfileService
**File:** `Services/UserProfileService.cs`

Manages user profile data and preferences:
- `GetUserProfileAsync(userId)` - Retrieve user profile
- `GetOrCreateUserProfileAsync(userId)` - Get or create if doesn't exist
- `UpdateUserProfileAsync(profile)` - Save profile changes
- `LinkApaAccountAsync(userId, refreshToken)` - Store APA refresh token
- `UnlinkApaAccountAsync(userId)` - Remove APA connection
- `IsApaLinkedAsync(userId)` - Check if APA is connected
- `GetApaRefreshTokenAsync(userId)` - Retrieve APA token
- `GetNotificationPreferencesAsync<T>(userId)` - Get typed preferences
- `SetNotificationPreferencesAsync<T>(userId, preferences)` - Save preferences

#### ApaLinkingService
**File:** `Services/ApaLinkingService.cs`

Handles APA OAuth flow and GraphQL integration:
- `GetApaAuthorizationUrl(redirectUri)` - Generate APA login URL
- `ExchangeRefreshTokenAsync(userId, refreshToken)` - Validate and store token
- `GetAccessTokenFromRefreshTokenAsync(refreshToken)` - Exchange for access token
- `GetAccessTokenForUserAsync(userId)` - Get token for authenticated user
- `ExecuteApaGraphQlQueryAsync<T>(userId, query, variables)` - Execute GraphQL with user's token

### 3. UI Components

#### Fixed User Menu
**File:** `Components/Shared/LoginDisplay.razor`

Rebuilt with:
- ✅ Reliable dropdown toggle
- ✅ Backdrop for click-outside detection
- ✅ Display name and email in header
- ✅ Links to Profile and Linked Accounts
- ✅ Sign out option
- ✅ Improved styling and animations

#### Profile Page
**File:** `Components/Pages/Profile.razor`
**Route:** `/profile`

Two sections:

**Identity Provider Information (Read-Only):**
- Full Name
- Email
- User ID
- Identity Provider (Microsoft, Google, Facebook, etc.)

**Local Profile Settings (Editable):**
- Nickname
- Preferred Display Name
- Time Zone (ET, CT, MT, PT)
- Notification Preferences:
  - Email notifications
  - Match reminders
  - Tournament alerts

Form validation and save functionality included.

#### Linked Accounts Page
**File:** `Components/Pages/LinkedAccounts.razor`
**Route:** `/linked-accounts`

Shows cards for:

1. **Microsoft Account** (Always connected)
   - Primary identity provider
   - Shows email address
   - Cannot be unlinked

2. **APA Account**
   - "Connect APA Account" button when not linked
   - Redirects to `https://accounts.poolplayers.com/login?redirect_uri=<encoded>`
   - Handles OAuth callback with `?apatoken=<refreshToken>`
   - Shows "Connected" status when linked
   - "Unlink Account" button when connected

3. **Facebook** (Coming Soon)
   - Placeholder for future implementation

4. **Google** (Coming Soon)
   - Placeholder for future implementation

### 4. Registration in Program.cs

Services registered:
```csharp
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddHttpClient<ApaLinkingService>();
```

## APA OAuth Flow

### 1. User Initiates Connection
User clicks "Connect APA Account" on `/linked-accounts` page

### 2. Redirect to APA
```
https://accounts.poolplayers.com/login?redirect_uri=https://localhost:7158/linked-accounts
```

### 3. User Authenticates
User logs in to APA account

### 4. APA Redirects Back
```
https://localhost:7158/linked-accounts?apatoken=<refreshToken>
```

### 5. Store Refresh Token
- `LinkedAccounts.razor` detects `apatoken` parameter
- Calls `ApaLinkingService.ExchangeRefreshTokenAsync()`
- Validates token by attempting to get access token
- Stores refresh token in `UserProfiles.ApaRefreshToken`

### 6. Using APA Data
```csharp
// Get access token for user
var accessToken = await apaLinking.GetAccessTokenForUserAsync(userId);

// Or execute GraphQL directly
var result = await apaLinking.ExecuteApaGraphQlQueryAsync<MyType>(
    userId,
    "query { viewer { id name } }",
    variables: null
);
```

## Security Considerations

### Refresh Token Storage
- Stored in database (consider encryption in production)
- Only accessible to owning user
- Can be revoked by unlinking account

### Access Token Handling
- Never stored, only exchanged on-demand
- Short-lived (managed by APA)
- Automatically refreshed using refresh token

### Identity Provider Claims
- All external identity data is read-only
- Cannot be modified through application
- Sourced directly from Microsoft Entra ID

## Testing Checklist

### User Menu
- [ ] Click avatar to open dropdown
- [ ] Click outside to close dropdown
- [ ] Navigate to Profile
- [ ] Navigate to Linked Accounts
- [ ] Sign out works

### Profile Page
- [ ] View identity provider information
- [ ] Edit local profile fields
- [ ] Toggle notification preferences
- [ ] Save changes successfully
- [ ] Validation works correctly

### Linked Accounts Page
- [ ] Microsoft account shows as connected
- [ ] APA shows "Not Connected" initially
- [ ] Click "Connect APA Account"
- [ ] Redirect to APA works
- [ ] Return from APA with token
- [ ] APA shows "Connected" after linking
- [ ] Can unlink APA account
- [ ] Coming soon badges show for Facebook/Google

### Database
- [ ] Run migration successfully
- [ ] UserProfiles table created
- [ ] Profile records created on first access
- [ ] Updates save correctly
- [ ] APA tokens stored/removed correctly

## Database Migration

```bash
# Navigate to DataSQL project
cd CueConnect757.DataSQL

# Create migration (already done)
dotnet ef migrations add AddUserProfiles --startup-project ../CueConnect757

# Apply migration to database
dotnet ef database update --startup-project ../CueConnect757

# To rollback
dotnet ef migrations remove --startup-project ../CueConnect757
```

## Usage Examples

### Check if User Has APA Linked
```csharp
@inject UserProfileService ProfileService

var isLinked = await ProfileService.IsApaLinkedAsync(userId);
if (isLinked)
{
    // Show APA stats
}
```

### Get User's APA Data
```csharp
@inject ApaLinkingService ApaLinking

var stats = await ApaLinking.ExecuteApaGraphQlQueryAsync<ApaStats>(
    userId,
    @"query { 
        viewer { 
            eightBallStats { 
                skillLevel 
                wins 
                losses 
            } 
        } 
    }"
);
```

### Save Custom Notification Preferences
```csharp
@inject UserProfileService ProfileService

var prefs = new NotificationPreferences
{
    EmailNotifications = true,
    MatchReminders = false,
    TournamentAlerts = true
};

await ProfileService.SetNotificationPreferencesAsync(userId, prefs);
```

## File Structure

```
CueConnect757/
├── Components/
│   ├── Pages/
│   │   ├── Profile.razor (new)
│   │   ├── LinkedAccounts.razor (new)
│   │   └── ...
│   └── Shared/
│       └── LoginDisplay.razor (updated)
├── Services/
│   ├── UserProfileService.cs (new)
│   └── ApaLinkingService.cs (new)
└── Program.cs (updated)

CueConnect757.DataSQL/
├── Entities/
│   └── UserProfile.cs (new)
├── Migrations/
│   └── [timestamp]_AddUserProfiles.cs (new)
└── CueConnectDbContext.cs (updated)
```

## Next Steps

1. **Apply Database Migration**
   ```bash
   cd CueConnect757.DataSQL
   dotnet ef database update --startup-project ../CueConnect757
   ```

2. **Test the Flow**
   - Sign in
   - Click avatar → Profile
   - Update profile information
   - Click avatar → Linked Accounts
   - Connect APA account (requires APA credentials)

3. **Security Enhancements (Production)**
   - Encrypt `ApaRefreshToken` in database
   - Add rate limiting to OAuth endpoints
   - Implement token expiration handling
   - Add logging for security events

4. **Future Enhancements**
   - Facebook authentication
   - Google authentication
   - BCA account linking
   - USAPL account linking
   - Profile picture upload
   - Privacy settings

## Troubleshooting

### Dropdown Not Opening
- Check browser console for JavaScript errors
- Verify `LoginDisplay.razor.css` is included
- Clear browser cache

### APA OAuth Not Working
- Verify redirect URI matches exactly in APA settings
- Check network tab for failed requests
- Ensure HTTPS is used (required for OAuth)

### Database Errors
- Verify connection string in `appsettings.Development.json`
- Run migration: `dotnet ef database update`
- Check SQL Server is running

### Token Exchange Failing
- Verify APA OAuth endpoint is accessible
- Check refresh token format
- Ensure HttpClient is configured correctly

## API Documentation

### APA OAuth Endpoints (External)
- **Authorization:** `https://accounts.poolplayers.com/login`
- **Token Exchange:** `https://accounts.poolplayers.com/oauth/token`
- **GraphQL:** `https://gql.poolplayers.com/graphql`

### Application Routes (Internal)
- `/profile` - User profile management
- `/linked-accounts` - External account linking
- `/linked-accounts?apatoken=<token>` - OAuth callback

## Build Status
✅ Build successful
✅ All services registered
✅ Migration created
✅ Components compiled
