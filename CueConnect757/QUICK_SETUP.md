# Quick Setup Guide

## 1. Get Your Azure AD Values

Go to the [Azure Portal](https://portal.azure.com) and navigate to:
**Azure Active Directory** > **App registrations** > **[Your App Name]**

### Get Tenant ID:
- Look at the **Overview** page
- Copy the **Directory (tenant) ID**

### Get Client ID:
- Look at the **Overview** page
- Copy the **Application (client) ID**

## 2. Update appsettings.Development.json

Replace `YOUR_TENANT_ID` and `YOUR_CLIENT_ID` with your actual values:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "12345678-1234-1234-1234-123456789abc",
    "ClientId": "87654321-4321-4321-4321-abcdefghijkl",
    "CallbackPath": "/signin-oidc",
    "SignedOutCallbackPath": "/signout-callback-oidc"
  }
}
```

## 3. Verify Redirect URIs in Azure Portal

1. Go to **Authentication** in your App registration
2. Under **Platform configurations** > **Web**, verify:
   - ✅ `https://localhost:7158/signin-oidc`
   - ✅ `https://cueconnect757.delfraisse.com/signin-oidc`
3. Under **Front-channel logout URL**, add:
   - `https://localhost:7158/signout-callback-oidc`

## 4. Configure Token Claims (Optional but Recommended)

1. Go to **Token configuration**
2. Click **Add optional claim**
3. Select **ID** token type
4. Check these boxes:
   - ✅ email
   - ✅ family_name
   - ✅ given_name
   - ✅ preferred_username
5. Click **Add**

## 5. Test the Setup

1. Run your app: `dotnet run`
2. Navigate to `https://localhost:7158`
3. Click **Sign in**
4. Login with your Microsoft account
5. After successful login, check:
   - Your name appears in the top-right
   - Visit `/account` to see all claims
   - Visit `/protected` to test authorization

## Common Issues

### "Reply URL does not match"
- Check that redirect URIs in Azure Portal match exactly
- No trailing slashes
- Must use HTTPS

### "Configuration could not be obtained"
- Verify Tenant ID and Client ID are correct
- Check internet connection
- Ensure app is not behind a strict firewall

### Missing email or name claims
- Add optional claims in Azure Portal (see step 4 above)
- Sign out and sign in again to get updated token

## Production Setup

For production at `https://cueconnect757.delfraisse.com`:

1. Update `appsettings.json` (NOT Development) with same Azure AD values
2. Ensure redirect URI `https://cueconnect757.delfraisse.com/signin-oidc` is in Azure Portal
3. Test thoroughly in production environment
4. Consider using Azure Key Vault or User Secrets for sensitive values

## Security Best Practices

- ✅ Never commit Tenant ID or Client ID to public repositories
- ✅ Use User Secrets for local development: `dotnet user-secrets set "AzureAd:ClientId" "your-value"`
- ✅ Use Azure Key Vault or App Configuration for production
- ✅ Regularly rotate secrets if using client secrets (not needed for this setup)
- ✅ Monitor sign-in logs in Azure AD
