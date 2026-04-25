# Microsoft Entra ID Authentication Setup

This document explains how to configure and use Microsoft Entra ID (Azure AD) authentication in CueConnect757.

## Prerequisites

1. Your app is already registered in Microsoft Entra ID
2. Redirect URIs configured:
   - Development: `https://localhost:7158/signin-oidc`
   - Production: `https://cueconnect757.delfraisse.com/signin-oidc`

## Required NuGet Packages

Add these packages to your project:

```bash
dotnet add package Microsoft.Identity.Web
dotnet add package Microsoft.Identity.Web.UI
```

## Configuration

### 1. Update appsettings.json

In your Azure Portal, find your App Registration and get:
- **Tenant ID**: Azure AD > App registrations > [Your App] > Overview > Directory (tenant) ID
- **Client ID**: Azure AD > App registrations > [Your App] > Overview > Application (client) ID

Update `appsettings.Development.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

For production, update `appsettings.json` with the same structure.

### 2. Verify Redirect URIs

In Azure Portal:
1. Go to Azure AD > App registrations > [Your App]
2. Click "Authentication"
3. Verify these redirect URIs are listed:
   - `https://localhost:7158/signin-oidc`
   - `https://cueconnect757.delfraisse.com/signin-oidc`
4. Under "Front-channel logout URL", add:
   - `https://localhost:7158/signout-callback-oidc`
   - `https://cueconnect757.delfraisse.com/signout-callback-oidc`

### 3. Token Configuration (Optional)

If you need access to email and other claims:

1. In Azure Portal, go to App registrations > [Your App] > Token configuration
2. Click "Add optional claim"
3. Select "ID" token type
4. Add these claims:
   - email
   - family_name
   - given_name
   - preferred_username

## Usage

### Protecting Pages

Add the `[Authorize]` attribute to any page that requires authentication:

```razor
@page "/protected"
@attribute [Authorize]
@using Microsoft.AspNetCore.Authorization

<h1>Protected Page</h1>
<p>Only authenticated users can see this.</p>
```

### Accessing User Information

#### Option 1: Using UserInfoService (Recommended)

```razor
@inject UserInfoService UserInfo

@code {
    private UserInfo? userInfo;

    protected override async Task OnInitializedAsync()
    {
        userInfo = await UserInfo.GetUserInfoAsync();
        
        if (userInfo.IsAuthenticated)
        {
            var name = userInfo.Name;
            var email = userInfo.Email;
            var userId = userInfo.UserId;
        }
    }
}
```

#### Option 2: Using AuthorizeView

```razor
<AuthorizeView>
    <Authorized>
        <p>Welcome, @context.User.Identity?.Name!</p>
        <p>Email: @context.User.FindFirst("preferred_username")?.Value</p>
    </Authorized>
    <NotAuthorized>
        <p>Please sign in.</p>
    </NotAuthorized>
</AuthorizeView>
```

#### Option 3: Using CascadingParameter

```razor
@code {
    [CascadingParameter]
    private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (AuthenticationStateTask != null)
        {
            var authState = await AuthenticationStateTask;
            var user = authState.User;
            
            if (user.Identity?.IsAuthenticated ?? false)
            {
                var name = user.Identity.Name;
                var email = user.FindFirst("preferred_username")?.Value;
            }
        }
    }
}
```

### Common Claims

- **Name**: `ClaimTypes.Name` or `"name"`
- **Email**: `ClaimTypes.Email` or `"preferred_username"`
- **User ID**: `ClaimTypes.NameIdentifier` or `"sub"`
- **Given Name**: `ClaimTypes.GivenName` or `"given_name"`
- **Surname**: `ClaimTypes.Surname` or `"family_name"`

## Testing

1. Run the application: `dotnet run`
2. Navigate to `https://localhost:7158`
3. Click "Sign in" in the top-right corner
4. You'll be redirected to Microsoft login
5. After successful login, you'll be redirected back to the app
6. Visit `/account` to see all your claims
7. Visit `/protected` to test a protected page

## Troubleshooting

### "AADSTS50011: The reply URL specified in the request does not match"

- Verify the redirect URI in Azure Portal matches exactly
- Check for trailing slashes
- Ensure HTTPS is used

### "Unable to obtain configuration from: https://login.microsoftonline.com/..."

- Check that TenantId and ClientId are correct
- Ensure your app has internet access
- Verify the Azure AD endpoint is accessible

### User claims are missing

- Go to Azure Portal > App registrations > [Your App] > Token configuration
- Add optional claims (email, given_name, family_name, preferred_username)
- Users may need to sign out and sign in again to get new claims

### SignOut not working

- Verify SignedOutCallbackPath is configured in appsettings.json
- Check that front-channel logout URL is configured in Azure Portal
- Clear browser cookies

## Components Created

- **LoginDisplay.razor**: Sign in/out button with user info
- **Account.razor**: Page showing all user claims
- **Protected.razor**: Example of a protected page
- **UserInfoService.cs**: Service for accessing user information

## Additional Resources

- [Microsoft.Identity.Web Documentation](https://learn.microsoft.com/azure/active-directory/develop/microsoft-identity-web)
- [Azure AD App Registration](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app)
- [ASP.NET Core Blazor Authentication](https://learn.microsoft.com/aspnet/core/blazor/security/)
